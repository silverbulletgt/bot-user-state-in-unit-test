using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotUserStateTest
{
    public class StateService
    {
        public UserState UserState { get; }
        public IStatePropertyAccessor<BotUserState> UserStateAccessor { get; set; }

        public StateService(UserState userState)
        {
            UserState = userState;
            InitializeAccessors();
        }

        private void InitializeAccessors()
        {
            UserStateAccessor = UserState.CreateProperty<BotUserState>(nameof(BotUserState));
        }
    }

    public class BotUserState
    {
        public string UserStateValue { get; set; }
    }
}
