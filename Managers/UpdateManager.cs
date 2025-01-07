using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static GlobalData;

public class UpdateManager
{
    private CharacterCreation characterCreation;
    private CharacterDisplay characterDisplay;
    private AttackEnemyHandler attackEnemyHandler;
    private ShopKeeper shopKeeper;
    private Dictionary<string, Func<Wrapper, Task>> callbackActions;
    private Dictionary<string, Func<Wrapper, Task>> callbackActionsRich;
    private CharacterTrade characterTrade;
    private CharacterItems characterItems; 
    private StashHandler stashHandler;
    private DungeonsHandler dungeonsHandler;
    private StoryHandler storyHandler;
    private ClassShop classShop;

    public UpdateManager()
    {
        characterCreation = new CharacterCreation();
        characterDisplay = new CharacterDisplay();
        attackEnemyHandler = new AttackEnemyHandler();
        shopKeeper = new ShopKeeper();
        characterTrade = new CharacterTrade();
        characterItems = new CharacterItems();
        stashHandler = new StashHandler();
        dungeonsHandler = new DungeonsHandler();
        storyHandler = new StoryHandler();
        classShop = new ClassShop();

        callbackActionsRich = new Dictionary<string, Func<Wrapper, Task>> 
        {
            { "@stash", async (wrapper) => await stashHandler.FindStash(wrapper) },
            { "@attack", async (wrapper) => await attackEnemyHandler.EnqueueAttackEnemy(wrapper)},
            { "@attackDangion", async (wrapper) => await attackEnemyHandler.EnqueueAttackEnemy(wrapper) },
            { "@dangion", async (wrapper) => await dungeonsHandler.DungeonsManage(wrapper) },
        };

        callbackActions = new Dictionary<string, Func<Wrapper, Task>>
        {
            { "give_gold", async (wrapper) => {
                await characterTrade.GiveGold(wrapper);
                SetNextCommand(wrapper.UserId, async (nextWrapper) => {
                await characterTrade.WaitingForRecipient(nextWrapper.OriginalMessage, nextWrapper.Type);
                    SetNextCommand(wrapper.UserId, async (nextWrapper) => {
                    await characterTrade.WaitingForAmount(nextWrapper.OriginalMessage, nextWrapper.Type);
                    });
                });
            }},
            { "show_shop", async (wrapper) => {
                await shopKeeper.ItemShop(wrapper);
                SetNextCommand(wrapper.UserId, async (nextWrapper) => 
                await shopKeeper.ChoseItem(nextWrapper));
            }},
            { "sell_shop", async (wrapper) => {await characterItems.SellItem(wrapper);
                 SetNextCommand(wrapper.UserId, async (nextWrapper) => {
                    await characterItems.WaitingForItemToSell(nextWrapper); 
                    });
                }},
            { "use_item", async (wrapper) => {await characterItems.UseItem(wrapper);
                SetNextCommand(wrapper.UserId, async (nextWrapper) => {
                    await characterItems.WaitingForItem(nextWrapper);
                    });
                }},
            { "remove_equipment", async (wrapper) => {await characterItems.RemoveItem(wrapper);
                SetNextCommand(wrapper.UserId, async (nextWrapper) => {
                    await characterItems.WaitingForItemRemove(nextWrapper);
                    });
            } },
            { "change_class", async (wrapper) => {await classShop.ChangeClass(wrapper);
                SetNextCommand(wrapper.UserId, async (nextWrapper) => {
                    await classShop.ChoseClass(nextWrapper);
                    });
            } },
            { "upgrade_item", async (wrapper) => {await characterItems.UseItem(wrapper);
                SetNextCommand(wrapper.UserId, async (nextWrapper) => {
                    await characterItems.UpgradeItem(nextWrapper); 
                    });
                }
            },
            { "trash_sell", async (wrapper) => await characterItems.SellAllTrash(wrapper) },
            { "back", async (wrapper) => await characterDisplay.ShowCharacterRefreshProxy(wrapper) },
            { "show_inventory", async (wrapper) => await characterDisplay.ShowInventoryEdit(wrapper) },
            { "refresh_name", async (wrapper) => await characterCreation.RefreshName(wrapper) },
            { "show_auras", async (wrapper) => await characterDisplay.ShowBuffs(wrapper) },
            { "close_hero", async (wrapper) => await characterDisplay.CloseHero(wrapper) },
            { "ACT_1_yes_CHOSE_1", async (wrapper) => await storyHandler.yes_ACT_1_CHOSE_1(wrapper) },
            { "ACT_1_no_CHOSE_1", async (wrapper) => await storyHandler.no_ACT_1_CHOSE_1(wrapper) },
            { "ACT_1_yes_CHOSE_2", async (wrapper) => await storyHandler.yes_ACT_1_CHOSE_2(wrapper) },
        };
    }
    private void SetNextCommand(long userId, Func<Wrapper, Task> nextAction) => MassageManager.nextCommand[userId] = nextAction;
    public async Task OnUpdate(Update update)
    {
        var wrappedUpdate = new Wrapper(update);

        if (update.Type == UpdateType.CallbackQuery && userCharacterData.ContainsKey(wrappedUpdate.UserId))
        {
            string response = wrappedUpdate.Text;

            if (response.StartsWith("@"))
            {
                string commandKey = response.Substring(0, response.IndexOf("_"));
                if (callbackActionsRich.TryGetValue(commandKey, out var actionRich))
                {
                    await actionRich(wrappedUpdate);
                    return;
                }
            }

            if (MassageManager.nextCommand.TryGetValue(wrappedUpdate.UserId, out var nextAction))
            {
                MassageManager.nextCommand.Remove(wrappedUpdate.UserId);
                _ = nextAction(wrappedUpdate);
                return;
            }
            if (callbackActions.TryGetValue(response, out var action) && wrappedUpdate.CallbackQuery != null)
            {
                await action(wrappedUpdate);
                return; 
            }
        }
        else if (wrappedUpdate.CallbackQuery != null && wrappedUpdate.CallbackQuery.Message != null)
        {
            Message msgNew2 = await BotServices.Instance.Bot.SendMessage(
                chatId: wrappedUpdate.ChatId,
                text: $"Создайте персонажа если хотите участвовать."
            );
            _ = MassageDeleter(msgNew2, 30);
            if (wrappedUpdate.CallbackQueryId != null)
                _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrappedUpdate.CallbackQueryId);
        }
    }
}