using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotUserStateTest.Dialogs
{
    public class SaveAndRetrieveUserStateDialog : ComponentDialog
    {
        private StateService _stateService;
        public SaveAndRetrieveUserStateDialog(StateService stateService)
        {
            _stateService = stateService;

            AddDialog(new WaterfallDialog($"{nameof(SaveAndRetrieveUserStateDialog)}.mainFlow", WaterfallSteps));

            AddDialog(new TextPrompt($"{nameof(SaveAndRetrieveUserStateDialog)}.waitPrompt"));

            InitialDialogId = $"{nameof(SaveAndRetrieveUserStateDialog)}.mainFlow";
        }

        protected WaterfallStep[] WaterfallSteps => new WaterfallStep[]
                {
                    SetUserActivityTextToUserState,
                    RespondWithUserActivityState
                };

        protected async Task<DialogTurnResult> SetUserActivityTextToUserState(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string userText = stepContext.Context.Activity.Text;

            //https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-state?view=azure-bot-service-4.0&tabs=csharp
            var userStateAccessors = _stateService.UserStateAccessor;
            var userProfile = userStateAccessors.GetAsync(stepContext.Context, () => new BotUserState()).Result;
            userProfile.UserStateValue = userText;

            return await stepContext.PromptAsync($"{nameof(SaveAndRetrieveUserStateDialog)}.waitPrompt", new PromptOptions() { Prompt = MessageFactory.Text("Say something else to retrieve the value.") });
        }

        protected async Task<DialogTurnResult> RespondWithUserActivityState(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-state?view=azure-bot-service-4.0&tabs=csharp
            var userStateAccessors = _stateService.UserStateAccessor;
            var userProfile = userStateAccessors.GetAsync(stepContext.Context, () => new BotUserState()).Result;

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(userProfile.UserStateValue));

            return await stepContext.EndDialogAsync();
        }
    }
}
