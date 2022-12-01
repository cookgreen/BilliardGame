using System;
using System.Collections.Generic;
using System.Text;
using org.ogre.framework;

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
            if (!OgreFramework.Instance.InitOgre("Billiards", "ballIcon.ico", "BilliardGame", "resources.cfg"))
		        return;

            OgreFramework.Instance.log.LogMessage("BilliardGame initialized!");

            appStateManager = new AppStateManager();

            AppState.Create<MenuState>(appStateManager, "MainMenu");
            AppState.Create<GameState>(appStateManager, "GameState");
            AppState.Create<GameNetworkState>(appStateManager, "NetworkState");

            appStateManager.Start(appStateManager.FindByName("MainMenu"));
        }

        public void Dispose()
        {
            appStateManager = null;
        }

        private AppStateManager appStateManager;
    }
}
