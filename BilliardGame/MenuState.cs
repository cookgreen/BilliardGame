using System;
using System.Collections.Generic;
using System.Text;
using Mogre;
using MOIS;
using Mogre_Procedural.MogreBites;
using Vector3 = Mogre.Vector3;
using org.ogre.framework;

namespace BilliardGame
{
    public class MenuState : AppState
    {
        private SceneNode ball1;
        private SceneNode ball2;

        public MenuState()
        {
            m_bQuit = false;
            frameEvent = new FrameEvent();
        }
        public override void Enter()
        {
            FontManager.Singleton.GetByName("SdkTrays/Caption").Load();

            OgreFramework.Instance.log.LogMessage("Entering MainMenu...");
            m_bQuit = false;
 
            sceneMgr = OgreFramework.Instance.root.CreateSceneManager(Mogre.SceneType.ST_GENERIC, "MainMenuSceneMgr");
            ColourValue cvAmbineLight = new ColourValue(0.7f, 0.7f, 0.7f);
            sceneMgr.AmbientLight = cvAmbineLight;
 
            camera = sceneMgr.CreateCamera("MainMenuCamera");
            camera.SetPosition(0,25,-50);
            Mogre.Vector3 vectorCameraLookat = new Mogre.Vector3(0, 0, 0);
            camera.LookAt(vectorCameraLookat);
            camera.NearClipDistance = 1;

            camera.AspectRatio = OgreFramework.Instance.viewport.ActualWidth / OgreFramework.Instance.viewport.ActualHeight;

            OgreFramework.Instance.viewport.Camera = camera;

            OgreFramework.Instance.trayMgr.showFrameStats(TrayLocation.TL_BOTTOMLEFT);
            OgreFramework.Instance.trayMgr.showLogo(TrayLocation.TL_BOTTOMRIGHT);
            OgreFramework.Instance.trayMgr.showCursor();

            OgreFramework.Instance.trayMgr.destroyAllWidgets();
            OgreFramework.Instance.trayMgr.createLabel(TrayLocation.TL_TOP, "lbGameTitle", "Billiard Game", 250);
            OgreFramework.Instance.trayMgr.createButton(TrayLocation.TL_CENTER, "btnEnterOnePlayerGame", "Singleplayer", 250);
            OgreFramework.Instance.trayMgr.createButton(TrayLocation.TL_CENTER, "btnEnterTwoPlayerGame", "Multiplayer", 250);
            OgreFramework.Instance.trayMgr.createButton(TrayLocation.TL_CENTER, "btnExit", "Exit Game", 250);

            OgreFramework.Instance.mouse.MouseMoved += mouseMoved;
            OgreFramework.Instance.mouse.MousePressed += mousePressed;
            OgreFramework.Instance.mouse.MouseReleased += mouseReleased;
            OgreFramework.Instance.keyboard.KeyPressed += keyPressed;
            OgreFramework.Instance.keyboard.KeyReleased += keyReleased;

            createScene();
        }

        public void createScene()
        {
            sceneMgr.SetSkyBox(true, "Examples/SpaceSkyBox", 5000);

            Light light = sceneMgr.CreateLight();
            light.Type = Light.LightTypes.LT_POINT;
            light.Position = new Vector3(-250, 200, 0);
            light.SetSpecularColour(255, 255, 255);

            //ps = sceneMgr.CreateParticleSystem("Fireworks1", "Examples/GreenyNimbus");
            SceneNode node = sceneMgr.RootSceneNode.CreateChildSceneNode(new Vector3(-250, 60, 0));
            
            Entity ball = sceneMgr.CreateEntity("sphere_t.mesh");
            ball.SetMaterialName("ball_0");
            ball1 = node.CreateChildSceneNode(new Vector3(0, 100, 0));
            ball1.CreateChildSceneNode(new Vector3(-1, 0, 0)).AttachObject(ball);
            ball1.Scale(30, 30, 30);
            //node.AttachObject(ps);

            //ps = sceneMgr.CreateParticleSystem("Fireworks2", "Examples/GreenyNimbus");
            node = sceneMgr.RootSceneNode.CreateChildSceneNode(new Vector3(250, -60, 0));

            ball = sceneMgr.CreateEntity("sphere_t.mesh");
            ball.SetMaterialName("ball_1");
            ball2 = node.CreateChildSceneNode(new Vector3(0, 100, 0));
            ball2.CreateChildSceneNode(new Vector3(1, 0, 0)).AttachObject(ball);
            ball2.Scale(30, 30, 30);
            //node.AttachObject(ps);
        }

        public override void Exit()
        {
            OgreFramework.Instance.log.LogMessage("Leaving MainMenu...");

            OgreFramework.Instance.mouse.MouseMoved -= mouseMoved;
            OgreFramework.Instance.mouse.MousePressed -= mousePressed;
            OgreFramework.Instance.mouse.MouseReleased -= mouseReleased;
            OgreFramework.Instance.keyboard.KeyPressed -= keyPressed;
            OgreFramework.Instance.keyboard.KeyReleased -= keyReleased;

            sceneMgr.DestroyCamera(camera);
            if (sceneMgr != null)
                OgreFramework.Instance.root.DestroySceneManager(sceneMgr);

            OgreFramework.Instance.trayMgr.clearAllTrays();
            OgreFramework.Instance.trayMgr.destroyAllWidgets();
            OgreFramework.Instance.trayMgr.setListener(null);
        }

        public bool keyPressed(KeyEvent keyEventRef)
        {
            if(OgreFramework.Instance.keyboard.IsKeyDown(MOIS.KeyCode.KC_ESCAPE))
            {
                m_bQuit = true;
                return true;
            }

            OgreFramework.Instance.KeyPressed(keyEventRef);
            return true;
        }
        public bool keyReleased(KeyEvent keyEventRef)
        {
            OgreFramework.Instance.KeyReleased(keyEventRef);
            return true;
        }

        public bool mouseMoved(MouseEvent evt)
        {
            if (OgreFramework.Instance.trayMgr.injectMouseMove(evt)) return true;
            return true;
        }
        public bool mousePressed(MouseEvent evt, MouseButtonID id)
        {
            if (OgreFramework.Instance.trayMgr.injectMouseDown(evt, id)) return true;
            return true;
        }
        public bool mouseReleased(MouseEvent evt, MouseButtonID id)
        {
            if (OgreFramework.Instance.trayMgr.injectMouseUp(evt, id)) return true;
            return true;
        }

        public override void buttonHit(Button button)
        {
            if (button.getName() == "btnExit")
            {
                m_bQuit = true;
            }
            else if (button.getName() == "btnEnterOnePlayerGame")
            {
                Globals.Instance.NumPlayer = 1;
                Globals.Instance.RoundScore.Clear();
                Globals.Instance.RoundScore.Add(0);
                changeAppState(findByName("GameState"));
            }
            else if (button.getName() == "btnEnterTwoPlayerGame")
            {
                Globals.Instance.NumPlayer = 2;
                Globals.Instance.RoundScore.Clear();
                Globals.Instance.RoundScore.Add(0);
                Globals.Instance.RoundScore.Add(0);
                Globals.Instance.TotalRound = 5;
                Globals.Instance.CurRound = 0;
                changeAppState(findByName("NetworkState"));
            }
        }

        public override void Update(double timeSinceLastFrame)
        {
            frameEvent.timeSinceLastFrame = (float)timeSinceLastFrame;
            OgreFramework.Instance.trayMgr.frameRenderingQueued(frameEvent);

            ball1.Yaw(new Radian(new Degree((float)timeSinceLastFrame / 10.0f)));
            ball2.Yaw(new Radian(new Degree((float)timeSinceLastFrame / 10.0f)));

            if (m_bQuit == true)
            {
                shutdown();
                return;
            }
        }

        protected bool m_bQuit;
    }
}
