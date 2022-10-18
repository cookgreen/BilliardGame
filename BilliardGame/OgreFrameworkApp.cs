using System;
using System.Collections.Generic;
using System.Text;

namespace BilliardGame
{
    public class OgreFrameworkApp : IDisposable
    {
        public OgreFrameworkApp()
        {
            appStateManager = null;
        }

        public void Start()
        {
            if (!OgreFramework.Instance.InitOgre("Billiards"))
		        return;

            OgreFramework.Instance.log.LogMessage("BilliardGame initialized!");

            appStateManager = new AppStateManager();

            AppState.Create<MenuState>(appStateManager, "MainMenu");
            AppState.Create<GameState>(appStateManager, "GameState");

            appStateManager.Start(appStateManager.FindByName("MainMenu"));
        }

        public void Dispose()
        {
            appStateManager = null;
        }

        private AppStateManager appStateManager;
    }
}
