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
using org.ogre.framework;
using System.Net.Sockets;
using Vector3 = Mogre.Vector3;
using System.Net;

namespace BilliardGame
{
    public class GameNetworkState : AppState
    {
        private float camPosOffset;
        private bool isQuit;
        private BilliardGame game;

        private Button btnConnect;
        private EditBox txtIPAddress;
        private EditBox txtPort;

        private TcpClient tcpClient;

        public GameNetworkState()
        {
            isQuit = false;
            camPosOffset = -3;
            game = new BilliardGame();

            tcpClient = new TcpClient();
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

            OgreFramework.Instance.mouse.MouseMoved += new MouseListener.MouseMovedHandler(mouseMoved);
            OgreFramework.Instance.mouse.MousePressed += new MouseListener.MousePressedHandler(mousePressed);
            OgreFramework.Instance.mouse.MouseReleased += new MouseListener.MouseReleasedHandler(mouseReleased);
            OgreFramework.Instance.keyboard.KeyPressed += keyPressed;
            OgreFramework.Instance.keyboard.KeyReleased += keyReleased;

            setupUI();

            //game.SetupGame(camera);
        }

        private void setupUI()
        {
            Export.CreateGUI();
            Export.SetRenderWindow(OgreFramework.Instance.renderWnd);
            Export.SetSceneManager(sceneMgr);
            Export.SetActiveViewport(0);

            PointerManager.Instance.Visible = true;

            var widgets = Gui.Instance.LoadLayout("networkUI.layout");
            
            Button btnConnect = widgets[0].FindWidget("btnConnect") as Button;
            btnConnect.EventMouseButtonClick += new Widget.HandleMouseButtonClick(BtnConnect_EventMouseButtonClick);

            txtIPAddress = widgets[0].FindWidget("txtIPAddress") as EditBox;
            txtPort = widgets[0].FindWidget("txtPort") as EditBox;

            txtIPAddress.EventEditTextChange += new EditBox.HandleEditTextChange(TxtIPAddress_EventEditTextChange);
        }

        private void TxtIPAddress_EventEditTextChange(EditBox _sender)
        {
        }

        private void BtnConnect_EventMouseButtonClick(Widget _sender)
        {
            IPAddress ipAddress;
            if (!IPAddress.TryParse(txtIPAddress.Caption, out ipAddress))
            {
                MessageBox msgBox = Gui.Instance.CreateWidget<MessageBox>("Message", new IntCoord(0, 0, 100, 100), Align.Left, "Back");
                msgBox.Caption = "Error";
                msgBox.SetMessageText("Invalid IPAddress!");
                msgBox.SetMessageStyle(MessageBoxStyle.IconWarning | MessageBoxStyle.Yes);
                msgBox.SetWindowFade(true);
                msgBox.SetVisibleSmooth(true);
                msgBox.AddButtonName("Yes");
                msgBox.SetMessageModal(true);
                msgBox.EventMessageBoxResult += MsgBox_EventMessageBoxResult;
            }

            string port = txtPort.Caption;
        }

        private void MsgBox_EventMessageBoxResult(MessageBox _sender, MessageBoxStyle _result)
        {
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

            MyGUI.Sharp.InputManager.Instance.InjectKeyPress((MyGUI.Sharp.KeyCode)(int)evt.key, evt.text);

            return true;
        }
        public bool keyReleased(KeyEvent evt)
        {
            if (game.IsInited)
            {
                game.InjectKeyReleased(evt);
            }
            MyGUI.Sharp.InputManager.Instance.InjectKeyRelease((MyGUI.Sharp.KeyCode)(int)evt.key);

            return true;
        }

        public bool mouseMoved(MouseEvent evt)
        {
            if (game.IsInited)
            {
                game.InjectMouseMoved(evt);
            }

            MyGUI.Sharp.InputManager.Instance.InjectMouseMove(evt.state.X.abs, evt.state.Y.abs, evt.state.Z.abs);

            return true;
        }
        public bool mousePressed(MouseEvent evt, MouseButtonID id)
        {
            if (game.IsInited)
            {
                game.InjectMousePressed(evt, id);
            }

            MouseButton msGUI;
            if (id == MouseButtonID.MB_Left)
            {
                msGUI = MouseButton.Left;
            }
            else if (id == MouseButtonID.MB_Right)
            {
                msGUI = MouseButton.Right;
            }
            else if (id == MouseButtonID.MB_Middle)
            {
                msGUI = MouseButton.Middle;
            }
            else
            {
                msGUI = (MouseButton)(int)id;
            }

            MyGUI.Sharp.InputManager.Instance.InjectMousePress(evt.state.X.abs, evt.state.Y.abs, msGUI);

            return true;
        }
        public bool mouseReleased(MouseEvent evt, MouseButtonID id)
        {
            if (game.IsInited)
            {
                game.InjectMouseReleased(evt, id);
            }

            MouseButton msGUI;
            if (id == MouseButtonID.MB_Left)
            {
                msGUI = MouseButton.Left;
            }
            else if (id == MouseButtonID.MB_Right)
            {
                msGUI = MouseButton.Right;
            }
            else if (id == MouseButtonID.MB_Middle)
            {
                msGUI = MouseButton.Middle;
            }
            else
            {
                msGUI = (MouseButton)(int)id;
            }

            MyGUI.Sharp.InputManager.Instance.InjectMouseRelease(evt.state.X.abs, evt.state.Y.abs, msGUI);

            return true;
        }

        public override void Update(double timeSinceLastFrame)
        {
            if (isQuit)
            {
                shutdown();
                return;
            }

            //game.Update(timeSinceLastFrame);
            //
            //if (game.IsGameOver)
            //{
            //    if (Globals.Instance.NumPlayer == 1)
            //    {
            //        changeAppState(findByName("MenuState"));
            //    }
            //    else
            //    {
            //        Globals.Instance.CurRound += 1;
            //
            //        if (Globals.Instance.CurRound == Globals.Instance.TotalRound)
            //        {
            //            changeAppState(findByName("MenuState"));
            //        }
            //        else
            //        {
            //            Exit();
            //            Enter();
            //        }
            //    }
            //    return;
            //}
        }
    }
}
