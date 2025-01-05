using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static GlobalData;
using static StringCollection;

public class MassageManager
{
    private CharacterCreation characterCreation;
    private CharacterDeletion characterDeletion;
    private CharacterDisplay characterDisplay;
    private CharacterTrade characterTrade;
    private CharacterItems characterItems;
    private StaffZone staffZone;
    private ShopKeeper shopKeeper;
    private Dictionary<string, Func<Wrapper, Task>> commandActions;

    public static Dictionary<long, Func<Wrapper, Task>> nextCommand = new Dictionary<long, Func<Wrapper, Task>>();
    public MassageManager()
    {
        characterCreation = new CharacterCreation();
        characterDeletion = new CharacterDeletion();
        characterDisplay = new CharacterDisplay();
        staffZone = new StaffZone();
        shopKeeper = new ShopKeeper();
        characterTrade = new CharacterTrade();
        characterItems = new CharacterItems();

        commandActions = new Dictionary<string, Func<Wrapper, Task>>
        {
            { "Создать персонажа", async (wrapper) => {
                await characterCreation.CreatClass(wrapper.OriginalMessage, wrapper.Type);
                SetNextCommand(wrapper.UserId, async (nextWrapper) => {
                    await characterCreation.HandleGenderSelection(nextWrapper.OriginalMessage, nextWrapper.Type);
                    SetNextCommand(wrapper.UserId, async (nextWrapper) => {
                        await characterCreation.HandleClassSelection(nextWrapper.OriginalMessage, nextWrapper.Type);
                        SetNextCommand(wrapper.UserId, async (nextWrapper) => {
                            await characterCreation.HandleCharacterDetails(nextWrapper.OriginalMessage, nextWrapper.Type);
                        });
                    });
                });
            }},
            { "Передать игроку", async (wrapper) => {
                await characterTrade.GiveGold(wrapper);
                SetNextCommand(wrapper.UserId, async (nextWrapper) => {
                await characterTrade.WaitingForRecipient(nextWrapper.OriginalMessage, nextWrapper.Type);
                    SetNextCommand(wrapper.UserId, async (nextWrapper) => {
                    await characterTrade.WaitingForAmount(nextWrapper.OriginalMessage, nextWrapper.Type);
                    });
                });
            }},
            { "Открыть магазин", async (wrapper) => {
                await shopKeeper.ItemShop(wrapper.OriginalMessage);
                SetNextCommand(wrapper.UserId, async (nextWrapper) => 
                    await shopKeeper.ChoseItem(nextWrapper.OriginalMessage, nextWrapper.Type));
            } },
            { "Использовать предмет", async (wrapper) => {
                await characterItems.UseItem(wrapper);
                SetNextCommand(wrapper.UserId, async (nextWrapper) =>
                    await characterItems.WaitingForItem(nextWrapper));
            } },
            { "Убрать предмет", async (wrapper) => {
                await characterItems.RemoveItem(wrapper);
                SetNextCommand(wrapper.UserId, async (nextWrapper) =>
                 await characterItems.WaitingForItemRemove(nextWrapper));
            } },
            { "Играть", async (wrapper) => await characterDisplay.ShowCharacter(wrapper.ChatId, wrapper.MessageThreadId, wrapper.UserId, wrapper.OriginalMessage) },
            { "Обновить имя персонажа", async (wrapper) => await characterCreation.RefreshName(wrapper) },
            { "Удалить персонажа", async (wrapper) => await characterDeletion.Delete(wrapper.OriginalMessage, wrapper.Type) },
            { "Мой инвентарь", async (wrapper) => await characterDisplay.ShowInventory(wrapper.OriginalMessage, wrapper.Type) },
            { "Моя экипировка", async (wrapper) => await characterDisplay.ShowEquippedItems(wrapper.OriginalMessage, wrapper.Type) },
            { "Показать статистику", async (wrapper) => await characterDisplay.ShowTierList(wrapper.OriginalMessage, wrapper.Type) },
            { "Показать мой ID", async (wrapper) => await characterDisplay.ShowID(wrapper.OriginalMessage, wrapper.Type) },
            { "Показать здоровье", async (wrapper) => await characterDisplay.ShowHP(wrapper.OriginalMessage, wrapper.Type) },
            { "Показать мои ауры", async (wrapper) => await characterDisplay.ShowBuffs(wrapper) }
        };

    }
    public static void SetNextCommand(long userId, Func<Wrapper, Task> nextAction)  
    {
        if (nextCommand.ContainsKey(userId)) 
            return;
        nextCommand[userId] = nextAction; 
    }
    public static readonly Func<Wrapper, Task> EmptyCommand = async (wrapper) => await Task.CompletedTask;

    public async Task OnMessage(Message msg, UpdateType type)
    {
        if (msg.From == null || msg.Text == null) return;

        if (userCharacterData.ContainsKey(msg.From.Id))
            _ = CharacterManager.UpdateEXP(userCharacterData[msg.From.Id], msg);

        var wrappedMSG = new Wrapper(msg, type);

        if (nextCommand.TryGetValue(msg.From.Id, out var nextAction))
        {
            nextCommand.Remove(msg.From.Id);
            _ = nextAction(wrappedMSG);
            return;
        }
        
        if (commandActions.TryGetValue(msg.Text, out var action))
        {
            _ = action(wrappedMSG);
            return; 
        }

        if (msg.MessageThreadId == m_forPhotos)
            await staffZone.PhotoSaver(msg, type);

        if (msg.Chat.Id == m_DevMainChat)
            await staffZone.MassageSender(msg, type);

        if (msg.Chat.Id == m_DevMainChat && msg.MessageThreadId == m_EnemyCreationRoom)
            await staffZone.SendEnemy(msg, type);
    }
}