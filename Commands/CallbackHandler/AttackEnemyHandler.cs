using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static GlobalData;
using static StringCollection;

public class AttackEnemyHandler
{
    private readonly Random _random;
    private readonly ItemDropCollection _itemDrop;
    private const int MaxCaptionLength = 1024;
    private ConcurrentQueue<Wrapper> updateQueue = new ConcurrentQueue<Wrapper>();
    private HashSet<long> userIdsInQueue = new HashSet<long>();

    public AttackEnemyHandler()
    {
        _random = new Random();
        _itemDrop = new ItemDropCollection();
        StartProcessingQueue();
    }
    private void StartProcessingQueue()
    {
        Task.Run(async () =>
        {
            while (!СancellationTokenGlobal.IsCancellationRequested)
            {
                await Task.Delay(1024);

                if (updateQueue.TryDequeue(out var wrapper))
                {
                    userIdsInQueue.Remove(wrapper.UserId);
                    _ = AttackEnemy(wrapper);
                }
                
            }
        });
    }
    public Task EnqueueAttackEnemy(Wrapper wrapper)
    {
        if (wrapper.CallbackQueryId == null) throw new ArgumentNullException(nameof(wrapper.CallbackQueryId));

        if (userIdsInQueue.Contains(wrapper.UserId))
        {
            _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId, "Вы нажимаете кнопку слишком быстро.");
            return Task.CompletedTask;
        }
        updateQueue.Enqueue(wrapper);
        _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
        return Task.CompletedTask;
    }

    async public Task AttackEnemy(Wrapper wrapper)
    {
        var callbackQuery = wrapper.CallbackQuery;
        if (callbackQuery == null || callbackQuery.Message == null || callbackQuery.Data == null || callbackQuery.Message.Caption == null || wrapper.CallbackQueryId == null) throw
                new ArgumentNullException(nameof(callbackQuery));

        int? messageThreadId = callbackQuery.Message.MessageThreadId;
        string response = callbackQuery.Data;
        var parts = response.Split('_');

        var enemyID = int.Parse(parts[2]);
        var attackTimeTicks = long.Parse(parts[3]);
        var buttonType = parts[4];

        var userId = callbackQuery.From.Id;
        var character = userCharacterData[userId];

        _ = BotServices.Instance.Bot.AnswerCallbackQuery(callbackQuery.Id);

        if (parts[0] == "@attackDangion" && !dangionList.Contains(userId))
        {
            Message msgNew4 = await BotServices.Instance.Bot.SendMessage(
                    callbackQuery.Message.Chat.Id,
                    $"{character.Name} Вы не участник этого рейда.",
                    messageThreadId: messageThreadId
                );
            _ = MassageDeleter(msgNew4, 30);
            _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
            return;
        }


        if (userCharacterData.ContainsKey(userId) && EnemyPool.TryGetValue(enemyID, out var enemy))
        {
            int attackPower = character.ATK;
            int characterLevel = character.Level;

            var lastAttackTime = new DateTime(attackTimeTicks);
            var timeSinceLastAttack = DateTime.Now - lastAttackTime;

            bool canAttack = timeSinceLastAttack.TotalMinutes > 20;

            bool range = AreLevelsInSameRange(characterLevel, enemy.Level);
            bool haveRange = true;

            if (characterLevel > 5 && enemy.Level < 6)
                haveRange = false;
            else if (characterLevel > 10 && enemy.Level < 11)
                haveRange = false;

            if (!haveRange && !canAttack)
            {
                Message msgNew2 = await BotServices.Instance.Bot.SendMessage(
                    callbackQuery.Message.Chat.Id,
                    $"{character.Name} Ваш уровень слишком высокий. \nВы не можете атаковать {enemy.Name}!\nПожалуйста, подождите 20 минут и Вы сможете помочь.\n Ваше здоровье {character.CurrentHP} / {character.HP}",
                    messageThreadId: messageThreadId
                );
                _ = MassageDeleter(msgNew2, 30);
                _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
                return;
            }
            if (enemy.HP < 0)
            {
                _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
                return;
            }

            if (!enemy.PlayersID.Contains(userId))
                enemy.PlayersID.Add(userId);

            if (buttonType == "Skill")
            {
                if(character.UniqueSkill == null)
                {
                    Message msgNew6 = await BotServices.Instance.Bot.SendMessage(
                    callbackQuery.Message.Chat.Id,
                    $"{character.Name}, У Вас нету навыка!",
                    messageThreadId: messageThreadId
                );

                    _ = MassageDeleter(msgNew6, 30);
                    _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
                    return;
                }

                await character.UniqueSkill!.InitSkill(character, enemy, wrapper.ChatId, _random);
            }

            if (buttonType == "Simple")
            { 

            bool IsPlayerAlive = await enemy.CalculateDamage(character, callbackQuery.Message.Chat.Id, _random);

            if (!IsPlayerAlive)
            {
                enemy.PlayersID.Remove(userId);
                character.CurrentHP = 1;

                bool hasBuff = character.GetActiveBuffs().Any(buff => buff.Name == "Проклятье понижения здоровья");

                if (!hasBuff)
                {
                    Buff healthReductionBuff = new Buff
                    {
                        Name = "Проклятье понижения здоровья",
                        ATKModifier = 0,
                        HPModifier = -character.HP / 2,
                        HPRegenModifier = 0,
                        Duration = TimeSpan.FromMinutes(15),
                        StartTime = DateTime.Now,
                        Message = $"{userCharacterData[userId].Name}, Ваше здоровье было восстановлено после проклятия поражения!",
                    };

                    character.ApplyBuff(healthReductionBuff);

                    Message msgNew4 = await BotServices.Instance.Bot.SendMessage(
                        callbackQuery.Message.Chat.Id,
                        $"{character.Name} Вы пали от руки {enemy.Name}!\nАландрия вернула Вас к жизни, но у этого были последствия...\nНа следующие 15 минут Ваше здоровье понижено в двое, как и востановление здорлаья!\nВаше здоровье {character.CurrentHP} / {character.HP}",
                        messageThreadId: messageThreadId
                    );

                    _ = Program.BuffsHandler(callbackQuery.Message.Chat.Id, messageThreadId, healthReductionBuff);

                    _ = MassageDeleter(msgNew4, 30);
                    _ = BotServices.Instance.Bot.AnswerCallbackQuery(callbackQueryId: callbackQuery!.Id, text: "" +
                    "☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️\n" +
                    "☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️\n" +
                    "☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️\n" +
                    "☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️\n" +
                    "☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️\n" +
                    "☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️\n" +
                    "☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️☠️\n" +
                    "", cacheTime: 20, showAlert: true);
                        return;
                }

                Message msgNew5 = await BotServices.Instance.Bot.SendMessage(
                    callbackQuery.Message.Chat.Id,
                    $"{character.Name}, Вы уже пали в бою и Вам не хватает здоровья.\nВаше здоровье {character.CurrentHP} / {character.HP}",
                    messageThreadId: messageThreadId
                );

                _ = MassageDeleter(msgNew5, 30);
                _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
                return;

            }

            }
            string levelScoup = "";

            if (enemy.Level <= 5)
                levelScoup = "1 - 5";
            else if (enemy.Level <= 10)
                levelScoup = "6 - 10";
            else if (enemy.Level <= 15)
                levelScoup = "11 - 15";

            string newCaption = $"{enemy.Name}\n" +
                $"Класс: {enemy.ClassName}\n" +
                $"Здоровье: {enemy.HP}/{enemy.MaxHP}\n" +
                $"Атака: {enemy.ATK}\n" +
                $"Зашита: {enemy.Armor}\n" +
                $"Уровень: {enemy.Level}\n\n" +
                $"{enemy.Caption}\n\n" +
                $"Награда за победу: \n{enemy.EXP} Опыта\n{enemy.Gold} Золота\n\n" +
                $"Враг для левела: {levelScoup}.";

            try
            {

                if (enemy.HP > 0)
                {
                    var sentMessage = await BotServices.Instance.Bot.EditMessageCaption(
                    chatId: callbackQuery.Message.Chat.Id,
                    messageId: enemy.MassageId,
                    caption: newCaption,
                    replyMarkup: callbackQuery.Message.ReplyMarkup
                    );

                    enemy.MassageId = sentMessage.MessageId;
                }

                if (enemy.HP < 0)
                {
                    if (!EnemyPool.ContainsKey(enemyID))
                    {
                        _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
                        return; 
                    }

                    EnemyPool.Remove(enemyID);
                    _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId, "Решающий удар.");
                    _ = BotServices.Instance.Bot.DeleteMessage(
                        chatId: m_mainChat,
                        messageId: enemy.LastRemainder
                    );

                    string victoryCaption = $"{enemy.Name} был(а) повержен(а)!\nУровень - {enemy.Level}\n{enemy.Caption}\n\nУчастники получают {enemy.EXP} опыта и {enemy.Gold} золота:\n";
                    string CaptionHolder = "";

                    foreach (var playerID in enemy.PlayersID)
                    {
                        var player = userCharacterData[playerID];

                        (int exp, int gold) = await player.GetLUCKAsync(new int[] { enemy.EXP, enemy.Gold }, luckEnemyLootImage, callbackQuery.Message.Chat.Id, _random);
                        await player.GetItemAsync(_itemDrop.CreateRandomItem(player.Level, _random), callbackQuery.Message.Chat.Id, _random);

                        if (player.Level < MAX_LEVEL)
                        {
                            player.EXP += exp;
                        }
                        player.Gold += gold;
                        CaptionHolder += $"{player.Name} - Уровень: {player.Level}\n Получил(а) - опыта {exp} и золота {gold}.\n";
                        _ = CharacterManager.UpdateEXP(player, callbackQuery.Message);
                    }


                    if ((victoryCaption.Length + CaptionHolder.Length) <= MaxCaptionLength)
                        victoryCaption += CaptionHolder;
                    else
                        victoryCaption = $"{enemy.Name} был(а) повержен(а)!\nУровень - {enemy.Level}\n\nУчастники получают {enemy.EXP} опыта и {enemy.Gold} золота:\n" + CaptionHolder;

                    Message msgNew2 = await BotServices.Instance.Bot.EditMessageCaption(
                        chatId: callbackQuery.Message.Chat.Id,
                        messageId: enemy.MassageId,
                        caption: victoryCaption
                    );


                    Message msgNew1 = await BotServices.Instance.Bot.SendMessage(
                        callbackQuery.Message.Chat.Id,
                        victoryCaption
                    );
                    enemy.PlayersID.Clear();

                    _ = MassageDeleter(msgNew1, 30);
                    _ = MassageDeleter(msgNew2, 30);

                    if (parts[0] == "@attackDangion")
                    {
                        if (dangionEnemyIndex < currentDangionEnemyIds.Length - 1)
                        {
                            dangionEnemyIndex++;
                            int enemyId = currentDangionEnemyIds[dangionEnemyIndex];
                            Enemy? enemyNew = dangionEnemyList.FirstOrDefault(e => e.EnemyId == enemyId);
                            if (enemyNew == null) throw new ArgumentNullException(nameof(enemyNew));

                            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                            {
                            InlineKeyboardButton.WithCallbackData("Attack", $"@attackDangion_{0}_{enemyNew.EnemyId}_{DateTime.Now.Ticks}")
                        });

                            _ = Program.SendEnemyCastom(0, 0, 5, enemyNew, inlineKeyboard);
                        }
                        else
                        {
                            int randomIndex = _random.Next(2);
                            string selectedImage = randomIndex == 0 ? dangionVictoryImage : dangionVictorySecoundImage;

                            dangionEnemyIndex = 0;
                            string playerNames = string.Join("\n", dangionList
                                .Where(id => userCharacterData.ContainsKey(id))
                                .Select(id => userCharacterData[id].Name));

                            _ = BotServices.Instance.Bot.SendPhoto(
                               chatId: callbackQuery.Message.Chat.Id,
                               photo: selectedImage,
                               caption: $"Подземелье было закрыто: \n{playerNames}",
                               messageThreadId: messageThreadId
                            );

                            _ = Program.ChestHandler(20, 30, "1 - 5");

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


        }

        _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId, "Враг уже погиб или Вы не участник.");

    }
    private bool AreLevelsInSameRange(int level1, int level2)
    {
        int range1 = (int)(Math.Floor((double)level1 - 1) / 5);
        int range2 = (int)(Math.Floor((double)level2 - 1) / 5);
        if (range1 > range2)
            return false;

        return range1 == range2 || range1 < range2;
    }
    
}