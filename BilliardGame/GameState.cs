using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mogre;
using Mogre.PhysX;
using Mogre_Procedural.MogreBites.Addons;
using MyGUI.Sharp;
using MyGUI.OgrePlatform;
using MOIS;
using System.Security.Cryptography;
using org.ogre.framework;
using Vector3 = Mogre.Vector3;

namespace BilliardGame
{
    public class GameState : AppState
    {
        private float camPosOffset;
        private bool isQuit;
        private BilliardGame game;

        public GameState()
        {
            isQuit = false;
            camPosOffset = -3;
            game = new BilliardGame();
        }

        public override void Enter()
        {
            OgreFramework.Instance.log.LogMessage("Entering Game...");

            sceneMgr = OgreFramework.Instance.root.CreateSceneManager(SceneType.ST_GENERIC, "GameSceneMgr");
            ColourValue cvAmbineLight = new ColourValue(0.6f, 0.6f, 0.6f);
            sceneMgr.AmbientLight = cvAmbineLight;

            sceneMgr.SetSkyBox(true, "Examples/SpaceSkyBox", 5000);
 
            camera = sceneMgr.CreateCamera("MainCamera");
            camera.Position = new Vector3(camPosOffset, 2, 0);
            camera.NearClipDistance = 0.1f;
            camera.FarClipDistance = 0;

            camera.AspectRatio = OgreFramework.Instance.viewport.ActualWidth / OgreFramework.Instance.viewport.ActualHeight;

            OgreFramework.Instance.viewport.Camera = camera;

            OgreFramework.Instance.trayMgr.hideCursor();

            OgreFramework.Instance.mouse.MouseMoved += mouseMoved;
            OgreFramework.Instance.mouse.MousePressed += mousePressed;
            OgreFramework.Instance.mouse.MouseReleased += mouseReleased;
            OgreFramework.Instance.keyboard.KeyPressed += keyPressed;
            OgreFramework.Instance.keyboard.KeyReleased += keyReleased;

            Export.CreateGUI();
            Export.SetRenderWindow(OgreFramework.Instance.renderWnd);
            Export.SetSceneManager(sceneMgr);
            Export.SetActiveViewport(0);

            game.SetupGame(camera);
        }

        public override void Exit()
        {
            game.Destroy(); 

            OgreFramework.Instance.log.LogMessage("Leaving Game...");

            if (sceneMgr != null)
            {
                sceneMgr.DestroyCamera(camera);
                OgreFramework.Instance.root.DestroySceneManager(sceneMgr);
            }
        }

        public bool keyPressed(KeyEvent evt)
        {
            if (game.IsInited)
            {
                game.InjectKeyPressed(evt);
            }

            return true;
        }
        public bool keyReleased(KeyEvent evt)
        {
            if (game.IsInited)
            {
                game.InjectKeyReleased(evt);
            }

            return true;
        }

        public bool mouseMoved(MouseEvent evt)
        {
            if (game.IsInited)
            {
                game.InjectMouseMoved(evt);
            }

            return true;
        }
        public bool mousePressed(MouseEvent evt, MouseButtonID id)
        {
            if (game.IsInited)
            {
                game.InjectMousePressed(evt, id);
            }

            return true;
        }
        public bool mouseReleased(MouseEvent evt, MouseButtonID id)
        {
            if (game.IsInited)
            {
                game.InjectMouseReleased(evt, id);
            }

            return true;
        }

        public override void Update(double timeSinceLastFrame)
        {
            if (isQuit)
            {
                shutdown();
                return;
            }

            game.Update(timeSinceLastFrame);

            if (game.IsGameOver)
            {
                if (Globals.Instance.NumPlayer == 1)
                {
                    changeAppState(findByName("MenuState"));
                }
                else
                {
                    Globals.Instance.CurRound += 1;

                    if (Globals.Instance.CurRound == Globals.Instance.TotalRound)
                    {
                        changeAppState(findByName("MenuState"));
                    }
                    else
                    {
                        Exit();
                        Enter();
                    }
                }
                return;
            }
        }
    }
}
