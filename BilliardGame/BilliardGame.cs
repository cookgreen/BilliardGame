using Mogre.PhysX;
using Mogre;
using MOIS;
using Mogre_Procedural.MogreBites.Addons;
using MyGUI.OgrePlatform;
using MyGUI.Sharp;
using org.ogre.framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vector3 = Mogre.Vector3;

namespace BilliardGame
{
    public class BilliardGame : IUserNotify, IUserContactReport, IUserTriggerReport
    {
        private SceneManager sceneMgr;
        private Camera camera;

        private EViewMode curViewMode;
        private StaticText showText;
        private StaticImage powerBar;
        private StaticImage powerCover;
        private StaticText beginCount;
        private bool canJudge;
        private SdkCameraMan sdkCameraMan;
        private bool isShift;
        private bool leftBD;
        private bool fire;
        private bool canHit;
        private bool freezeTime;
        private bool init;
        private float initTime;
        private int numAwake;
        private int numGameLevel;
        private float powerStep;
        private float forwardLimit;
        private float backLimit;
        private float camPosOffset;
        private Physics physx;
        private Scene physxScene;
        private Mogre.PhysX.Material defaultPhysxMat;

        private Ball whiteBall;
        private Desk desk;
        private Cue cue;

        private Dictionary<Actor, Ball> ballMap;
        private int maxPower;
        private int maxBarLen;
        private float scalar;
        private float ballHeight;
        private bool isGameOver;
        private bool timeOver;
        private bool timeOverEnd;

        private int genWidth;
        private int genHeight;
        private float timeElapse;
        private int power;
        private int dirFlag;
        private float totalSeconds;
        private float overTime;

        private ManualObject manual;
        private SceneNode fireLine;

        private int playerIndex;
        private List<Widget> widgets;
        private List<ActorSceneNode> actorNodes;

        private Random rand;

        public List<Player> Players;
        public Player CurrentPlayer;
        public Player NextPlayer;

        public bool IsInited
        {
            get { return init; }
        }

        public bool IsGameOver
        {
            get { return isGameOver; }
        }

        public BilliardGame()
        {
            maxBarLen = 400;
            maxPower = 2100;
            genWidth = 30;
            genHeight = 62;
            ballHeight = 2.5f;

            Players = new List<Player>();
            ballMap = new Dictionary<Actor, Ball>();
            rand = new Random();

            scalar = (float)maxBarLen / (float)maxPower;
        }

        public void Reset()
        {
            canJudge = false;
            isShift = false;
            leftBD = false;
            fire = false;
            canHit = true;
            freezeTime = false;
            isGameOver = false;
            power = 0;
            dirFlag = 1;
            numGameLevel = 0;
            playerIndex = 0;
            totalSeconds = 600;
            init = false;
            initTime = 5;
            timeOver = false;
            overTime = 10;
            timeOverEnd = false;
            curViewMode = EViewMode.FOLLOW;

            camPosOffset = -3;
            forwardLimit = 5;
            backLimit = 20;
            powerStep = 700;
        }

        public void SetupGame(Camera camera)
        {
            this.camera = camera;
            sceneMgr = camera.SceneManager;

            Reset();

            setupUI();
            setupPlayer();
            setupPhysics();
            createScene();
        }

        private void createScene()
        {
            MeshManager.Singleton.CreatePlane("floor", ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME,
                new Plane(Vector3.UNIT_Y, 0), 1000, 1000, 1, 1, true, 1, 1, 1, Vector3.UNIT_Z);
            Entity floor = sceneMgr.CreateEntity("Floor", "floor");
            floor.SetMaterialName("BaseWhite");
            floor.CastShadows = false;
            sceneMgr.RootSceneNode.CreateChildSceneNode().AttachObject(floor);
            sceneMgr.ShadowTechnique = ShadowTechnique.SHADOWTYPE_STENCIL_ADDITIVE;

            Light light = sceneMgr.CreateLight();
            light.Type = Light.LightTypes.LT_POINT;
            light.Position = new Vector3(-10, 40, 20);
            light.SetSpecularColour(255, 255, 255);
            light.CastShadows = true;

            cue = new Cue("newcue");
            cue.Initialise(sceneMgr);
            cue.SetTargetPos(whiteBall.GetGlobalPosition());

#if DEBUG
            sdkCameraMan = new SdkCameraMan(camera);
            sdkCameraMan.setStyle(CameraStyle.CS_MANUAL);
#endif
            cue.CueNode.AttachObject(camera);
            camera.LookAt(whiteBall.GetGlobalPosition());

            fireLine = sceneMgr.RootSceneNode.CreateChildSceneNode("FireLine");
            manual = sceneMgr.CreateManualObject("line");
            fireLine.AttachObject(manual);
        }

        private void setupUI()
        {
            PointerManager.Instance.Visible = false;

            widgets = Gui.Instance.LoadLayout("playui.layout");

            beginCount = widgets.Where(o => o.GetName() == "begintime").FirstOrDefault() as StaticText;
            showText = widgets.Where(o => o.GetName() == "txt_time").FirstOrDefault() as StaticText;

            StaticImage powerFrame = Gui.Instance.CreateWidget<StaticImage>("StaticImage", new IntCoord(10, 100, 60, 400), Align.Default, "Overlapped");
            powerFrame.SetImageTexture("powerFrame.png");
            powerCover = powerFrame.CreateWidget<StaticImage>("StaticImage", new IntCoord(5, 5, 51, 0), Align.Default);

            powerBar = powerCover.CreateWidget<StaticImage>("StaticImage", new IntCoord(0, 0, 51, 390), Align.Default);
            powerBar.SetImageTexture("powerbar.png");

            Gui.Instance.CreateWidget<StaticImage>("StaticImage", new IntCoord(0, 0, 800, 64), Align.Default, "Overlapped").SetImageTexture("playerUI.png");
        }

        private void setupPlayer()
        {
            int numPlayer = Globals.Instance.NumPlayer;

            Player player = new Player("player1");
            PlayerUI playerUI = new PlayerUI();
            playerUI.TxtName = widgets.Where(o => o.GetName() == "player1_name").FirstOrDefault() as StaticText;
            playerUI.ImageGreen = widgets.Where(o => o.GetName() == "player1_green").FirstOrDefault() as StaticImage;
            playerUI.ImageRed = widgets.Where(o => o.GetName() == "player1_red").FirstOrDefault() as StaticImage;
            playerUI.TxtScores = widgets.Where(o => o.GetName() == "player1_scores").FirstOrDefault() as StaticText;
            playerUI.TxtRoundScores = widgets.Where(o => o.GetName() == "player1_bigscore").FirstOrDefault() as StaticText;
            player.playerUI = playerUI;
            playerUI.SetPlayerName(player.Name);
            Players.Add(player);

            player = new Player("player2");
            playerUI = new PlayerUI();
            playerUI.TxtName = widgets.Where(o => o.GetName() == "player2_name").FirstOrDefault() as StaticText;
            playerUI.ImageGreen = widgets.Where(o => o.GetName() == "player2_green").FirstOrDefault() as StaticImage;
            playerUI.ImageRed = widgets.Where(o => o.GetName() == "player2_red").FirstOrDefault() as StaticImage;
            playerUI.TxtScores = widgets.Where(o => o.GetName() == "player2_scores").FirstOrDefault() as StaticText;
            playerUI.TxtRoundScores = widgets.Where(o => o.GetName() == "player2_bigscore").FirstOrDefault() as StaticText;
            player.playerUI = playerUI;
            playerUI.SetPlayerName(player.Name);
            Players.Add(player);

            if (numPlayer == 1)
            {
                playerUI.SetPlayerName(" ");
            }

            CurrentPlayer = Players[playerIndex];

            if (numPlayer == 2)
            {
                for (int i = 0; i < Players.Count; i++)
                {
                    Players[i].SetTurn(false);
                    Players[i].SetRoundScore(Globals.Instance.RoundScore[i]);
                }

                CurrentPlayer.SetTurn(true);
            }

            SetGameLevel(0);

            ItemManager.Instance.CreateItem(EColorType.BROWN, ((StaticText)widgets.Where(o => o.GetName() == "txt_brown").FirstOrDefault()));
            ItemManager.Instance.CreateItem(EColorType.BLUE, ((StaticText)widgets.Where(o => o.GetName() == "txt_blue").FirstOrDefault()));
            ItemManager.Instance.CreateItem(EColorType.GREEN, ((StaticText)widgets.Where(o => o.GetName() == "txt_green").FirstOrDefault()));
            ItemManager.Instance.CreateItem(EColorType.BLACK, ((StaticText)widgets.Where(o => o.GetName() == "txt_black").FirstOrDefault()));
            ItemManager.Instance.CreateItem(EColorType.MSG, ((StaticText)widgets.Where(o => o.GetName() == "txt_win").FirstOrDefault()));

            ItemShowMsg itemMsg = ItemManager.Instance.GetItemByColor(EColorType.MSG) as ItemShowMsg;
            itemMsg.ImageBox = widgets.Where(o => o.GetName() == "win_image").FirstOrDefault() as StaticImage;
        }

        private void setupPhysics()
        {
            physx = Physics.Create();
            SceneDesc desc = new SceneDesc();
            desc.Gravity = new Vector3(0, -9.8f, 0);
            physxScene = physx.CreateScene(desc);

            physxScene.UserNotify = this;
            physxScene.UserTriggerReport = this;
            physxScene.UserContactReport = this;

            defaultPhysxMat = physxScene.Materials[0];
            defaultPhysxMat.Restitution = 0.5f;
            defaultPhysxMat.StaticFriction = 0.9f;
            defaultPhysxMat.DynamicFriction = 0.2f;

            PlaneShapeDesc planeShapeDesc = new PlaneShapeDesc(new Vector3(0, 1, 0), 0);
            planeShapeDesc.MaterialIndex = defaultPhysxMat.Index;
            physxScene.CreateActor(new ActorDesc(planeShapeDesc));

            actorNodes = new List<ActorSceneNode>();

            MaterialPtr redMat = MaterialManager.Singleton.Create("RedMat", "General");
            redMat.SetDiffuse(255, 0, 0, 1);

            whiteBall = new Ball(EColorType.WHITE, new Vector3(0, ballHeight, 0));
            whiteBall.Initialise(physxScene, sceneMgr);

            actorNodes.Add(whiteBall.BallActorSceneNode);
            ballMap.Add(whiteBall.BallActorSceneNode.Actor, whiteBall);

            GenerateBalls();
            desk = new Desk("testdesk", new Vector3(0, 0, 0));
            desk.Initialise(physxScene, sceneMgr);
            actorNodes.AddRange(desk.DeskParts);

            ActorDesc coverActorDesc = new ActorDesc();
            coverActorDesc.Body = new BodyDesc();
            coverActorDesc.Body.Mass = 100.0f;
            BoxShapeDesc boxShapeDesc = new BoxShapeDesc(
                new Vector3(100, 1, 100), new Vector3(0, 3.5f, 0));
            coverActorDesc.Shapes.Add(boxShapeDesc);
            Actor coverActor = physxScene.CreateActor(coverActorDesc);
        }

        public Ball GetBallByActor(Actor actor)
        {
            if (ballMap.ContainsKey(actor))
            {
                return ballMap[actor];
            }

            return null;
        }

        public void OnTrigger(Shape TriggerShape, Shape CollisionShape, TriggerFlags Status)
        {
            if (Status == TriggerFlags.TriggerOnEnter)
            {
                Actor actor = CollisionShape.Actor;
                for (int i = ballMap.Count - 1; i >= 0; i--)
                {
                    var kpl = ballMap.ElementAt(i);

                    if (kpl.Key == actor)
                    {
                        EColorType num = kpl.Value.ColorType;
                        CurrentPlayer.KickInBall(num);

                        if (num == EColorType.WHITE)
                        {
                            Vector3 pos = GetRandomPosition();
                            kpl.Key.GlobalPosition = pos;
                            kpl.Key.PutToSleep();
                            return;
                        }

                        if (num == EColorType.RED)
                        {
                            if (!actor.IsSleeping)
                            {
                                numAwake -= 1;
                            }

                            SceneNode sceneNode = kpl.Value.BallActorSceneNode.SceneNode;
                            sceneNode.SetVisible(false);

                            actorNodes.Remove(kpl.Value.BallActorSceneNode);
                            ballMap.Remove(actor);
                            kpl.Value.BallActorSceneNode.Actor.Dispose();
                        }
                        else
                        {
                            if (numGameLevel == 0)
                            {
                                Vector3 randPos = GetRandomPosition();
                                kpl.Key.GlobalPosition = randPos;
                                kpl.Key.PutToSleep();
                            }
                            else if (numGameLevel == 1)
                            {
                                numAwake -= 1;

                                SceneNode sceneNode = kpl.Value.BallActorSceneNode.SceneNode;
                                sceneMgr.DestroySceneNode(sceneNode);

                                actorNodes.Remove(kpl.Value.BallActorSceneNode);
                                ballMap.Remove(actor);
                                kpl.Value.BallActorSceneNode.Actor.Dispose();
                            }
                        }
                    }
                }
            }
        }

        public bool IsAllBallIn()
        {
            if (ballMap.Count == 1)
            {
                var kpl = ballMap.ElementAt(0);
                if (kpl.Value.ColorType == EColorType.WHITE)
                {
                    return true;
                }
            }

            return false;
        }

        public Player GetWinner()
        {
            if (Players[0].Score > Players[1].Score)
            {
                return Players[0];
            }
            else
            {
                return Players[1];
            }
        }

        public void OnSleep(ReadOnlyCollection<Actor> Actors)
        {
            if (canJudge)
            {
                numAwake -= Actors.Count;

                if (numAwake == 0)
                {
                    bool changeTurn = CurrentPlayer.Judge(this);

                    if (IsAllBallIn())
                    {
                        if (Globals.Instance.NumPlayer == 1)
                        {
                            isGameOver = true;
                        }
                        else
                        {
                            if (Players[0].Score > Players[1].Score)
                            {
                                Globals.Instance.RoundScore[0]++;
                            }
                            else
                            {
                                Globals.Instance.RoundScore[1]++;
                            }
                            ItemManager.Instance.Action(EColorType.MSG, this, 5);
                        }
                    }

                    if (changeTurn && (Globals.Instance.NumPlayer > 1))
                    {
                        playerIndex = (++playerIndex) % Players.Count;
                        CurrentPlayer.SetTurn(false);
                        CurrentPlayer = Players[playerIndex];
                        CurrentPlayer.SetTurn(true);
                    }

                    cue.SetVisible(true);
                    canHit = true;
                }
            }
        }

        public void OnWake(ReadOnlyCollection<Actor> Actors)
        {
            if (canJudge)
            {
                numAwake += Actors.Count;
            }
        }

        public void OnContactNotify(ContactPair Pair, ContactPairFlags Events)
        {
            if (Pair.IsDeletedFirst || Pair.IsDeletedSecond)
                return;

            Actor firstActor = Pair.ActorFirst;
            Ball ball = GetBallByActor(firstActor);

            if (ball != null)
            {
                CurrentPlayer.HitBall(ball.ColorType);
            }
        }

        public Vector3 GetRandomPosition()
        {
            Vector3 randPos = new Vector3();
            Vector3 ballPos = new Vector3();
            Vector3 actorPos;
            randPos.y = ballHeight;

            randPos.z = rand.Next(-genWidth / 2, genWidth / 2);
            randPos.x = rand.Next(-genHeight / 2, genHeight / 2);

            bool canHit = true;

            while (canHit)
            {
                if (ballMap.Count == 0)
                {
                    break;
                }

                for (int i = 0; i < ballMap.Count; i++)
                {
                    var kpl = ballMap.ElementAt(i);

                    actorPos = kpl.Key.GlobalPosition;
                    ballPos = actorPos;

                    if (ballPos.PositionEquals(randPos, 0.6f))
                    {
                        canHit = true;

                        randPos.z = rand.Next(-genWidth / 2, genWidth / 2);
                        randPos.x = rand.Next(-genHeight / 2, genHeight / 2);

                        break;
                    }
                }

                canHit = false;
            }

            return randPos;
        }

        public void GenerateBalls()
        {
            Ball ball;
            Vector3 randPos;

            for (int i = 0; i < 15; i++)
            {
                randPos = GetRandomPosition();
                ball = new Ball(EColorType.RED, randPos);
                ball.Initialise(physxScene, sceneMgr);

                actorNodes.Add(ball.BallActorSceneNode);
                ballMap.Add(ball.BallActorSceneNode.Actor, ball);
            }

            for (int i = 0; i < 2; i++)
            {
                randPos = GetRandomPosition();
                ball = new Ball(EColorType.GREEN, randPos);
                ball.Initialise(physxScene, sceneMgr);

                actorNodes.Add(ball.BallActorSceneNode);
                ballMap.Add(ball.BallActorSceneNode.Actor, ball);
            }

            for (int i = 0; i < 2; i++)
            {
                randPos = GetRandomPosition();
                ball = new Ball(EColorType.BROWN, randPos);
                ball.Initialise(physxScene, sceneMgr);

                actorNodes.Add(ball.BallActorSceneNode);
                ballMap.Add(ball.BallActorSceneNode.Actor, ball);
            }

            for (int i = 0; i < 2; i++)
            {
                randPos = GetRandomPosition();
                ball = new Ball(EColorType.BLUE, randPos);
                ball.Initialise(physxScene, sceneMgr);

                actorNodes.Add(ball.BallActorSceneNode);
                ballMap.Add(ball.BallActorSceneNode.Actor, ball);
            }

            for (int i = 0; i < 2; i++)
            {
                randPos = GetRandomPosition();
                ball = new Ball(EColorType.BLACK, randPos);
                ball.Initialise(physxScene, sceneMgr);

                actorNodes.Add(ball.BallActorSceneNode);
                ballMap.Add(ball.BallActorSceneNode.Actor, ball);
            }
        }

        public void DrawLine(Vector3 beginPos, Vector3 endPos)
        {
            manual.Clear();
            manual.Begin("BaseWhiteNoLighting", RenderOperation.OperationTypes.OT_LINE_LIST);
            manual.Position(beginPos);
            manual.Position(endPos);
            manual.End();
        }

        public int GetNumRedBall()
        {
            return ballMap.Where(o => o.Value.ColorType == EColorType.RED).Count();
        }

        public void SetGameLevel(int level)
        {
            numGameLevel = level;
            for (int i = 0; i < Players.Count; i++)
            {
                Players[i].SetGameLevel(level);
            }
        }

        public void Destroy()
        {
            Export.DestroyGUI();

            Players.Clear();
            ballMap.Clear();
            ItemManager.Instance.Reset();

            physxScene.Dispose();
            physx.Dispose();
        }

        public void SetViewMode(EViewMode mode)
        {
            if (curViewMode != mode)
            {
                if (mode == EViewMode.FOLLOW)
                {
                    camera.SetPosition(camPosOffset, 2, 0);
                    cue.CueNode.AttachObject(camera);
                    camera.LookAt(whiteBall.GetGlobalPosition());
                }
                else if (mode == EViewMode.GOD)
                {
                    cue.CueNode.DetachObject(camera);
                    camera.SetPosition(0, 67, 0);
                    camera.LookAt(0.0001f, 0.0001f, 0.0001f);
                    camera.Roll(new Radian(new Degree(-45)));
                }

                curViewMode = mode;
            }
        }

        public bool InjectKeyPressed(KeyEvent evt)
        {
            if (evt.key == MOIS.KeyCode.KC_ESCAPE)
            {
                isGameOver = true;
            }
            if (evt.key == MOIS.KeyCode.KC_SPACE)
            {
                Vector3 camPos = camera.Position;
                Vector3 dir = camera.Direction;

                BodyDesc bodyDesc = new BodyDesc();
                bodyDesc.Mass = 20.0f;

                ActorDesc actorDesc = new ActorDesc();
                SphereShapeDesc sphereShapeDesc = new SphereShapeDesc(0.5f);
                sphereShapeDesc.MaterialIndex = defaultPhysxMat.Index;
                actorDesc.Shapes.Add(sphereShapeDesc);
                actorDesc.Body = bodyDesc;
                Actor actor = physxScene.CreateActor(actorDesc);

                Entity ent = sceneMgr.CreateEntity("CameraCube_" + Guid.NewGuid().ToString(), "sphere_t.mesh");
                SceneNode sceneNode = sceneMgr.RootSceneNode.CreateChildSceneNode();
                sceneNode.AttachObject(ent);

                actor.GlobalPosition = camPos;
                ActorSceneNode actorSceneNode = new ActorSceneNode(actor, sceneNode);
                actorNodes.Add(actorSceneNode);

                actor.AddForce(dir, ForceModes.Impulse);
            }

            if (evt.key == MOIS.KeyCode.KC_LSHIFT)
            {
                isShift = true;
            }

            if (sdkCameraMan != null)
            {
                sdkCameraMan.injectKeyDown(evt);
            }

            return true;
        }
        public bool InjectKeyReleased(KeyEvent evt)
        {
            if (evt.key == MOIS.KeyCode.KC_LSHIFT)
            {
                isShift = false;
            }
            if (sdkCameraMan != null)
            {
                sdkCameraMan.injectKeyUp(evt);
            }

            return true;
        }

        public bool InjectMouseMoved(MouseEvent evt)
        {
            MouseEvent orientedEvt = evt;

            if (!fire && !leftBD)
            {
                float scalar = 0.5f;
                if (isShift)
                {
                    scalar = 0.1f;
                }
                cue.Rotate(Vector3.UNIT_Y, new Degree(-evt.state.X.rel * scalar));
            }

            if (canHit && !fire && !leftBD)
            {
                Vector3 dir = cue.GetDirUnder(-Vector3.UNIT_X);
                dir.Normalise();
                Vector3 startPos = whiteBall.GetGlobalPosition() + dir * 0.6f;

                Ray ray = new Ray(startPos, dir);
                RaycastHit hit;
                physxScene.RaycastClosestShape(ray, ShapesTypes.All, out hit);

                if (hit.Shape.Actor != null)
                {
                    DrawLine(startPos, startPos + dir * hit.Distance);
                }
            }
            else
            {
                manual.Clear();
            }

            if (orientedEvt.state.Z.rel != 0 && (curViewMode == EViewMode.FOLLOW))
            {
                float distToTarget = camera.Position.x;
                float height = camera.Position.y;

                distToTarget += orientedEvt.state.Z.rel / 120.0f;

                if (distToTarget < -backLimit)
                {
                    distToTarget = backLimit;
                }
                else if (distToTarget > forwardLimit)
                {
                    distToTarget = forwardLimit;
                }
                camera.SetPosition(distToTarget, height, 0);
            }

            if (sdkCameraMan != null)
            {
                sdkCameraMan.injectMouseMove(evt);
            }

            return true;
        }
        public bool InjectMousePressed(MouseEvent evt, MouseButtonID id)
        {
            if (id == MouseButtonID.MB_Left)
            {
                if (!fire && canHit && init && !timeOver)
                {
                    canJudge = true;
                    leftBD = true;
                    timeElapse = 0;
                    dirFlag = 1;
                    power = 0;
                    cue.SetCuePos(new Vector3(cue.DistToTarget, 0, 0));
                    powerCover.SetVisible(true);
                }
            }

            if (sdkCameraMan != null)
            {
                sdkCameraMan.injectMouseDown(evt, id);
            }

            return true;
        }
        public bool InjectMouseReleased(MouseEvent evt, MouseButtonID id)
        {
            if (id == MouseButtonID.MB_Left)
            {
                if (leftBD)
                {
                    fire = true;
                    leftBD = false;
                    manual.Clear();
                    powerCover.SetVisible(false);
                }
            }
            else if (id == MouseButtonID.MB_Right)
            {
                if (curViewMode == EViewMode.FOLLOW)
                {
                    SetViewMode(EViewMode.GOD);
                }
                else if (curViewMode == EViewMode.GOD)
                {
                    SetViewMode(EViewMode.FOLLOW);
                }
            }

            if (sdkCameraMan != null)
            {
                sdkCameraMan.injectMouseUp(evt, id);
            }

            return true;
        }

        public void ShowTime(float seconds)
        {
            int minute = (int)seconds / 60;
            int sec = (int)seconds % 60;

            string timeText = string.Empty;
            if (minute < 10)
            {
                timeText += "0";
            }

            timeText += minute.ToString() + ":";

            if (sec < 10)
            {
                timeText += "0";
            }

            timeText += sec.ToString();

            showText.SetCaption(timeText);
        }

        public void Update(double timeSinceLastFrame)
        {
            float timeSeconds = (float)timeSinceLastFrame;

            if (!init)
            {
                initTime -= timeSeconds;
                beginCount.SetCaption(initTime.ToString());

                if (initTime <= 0)
                {
                    init = true;
                    beginCount.SetVisible(false);
                }
            }

            if (!freezeTime && init && totalSeconds > 0 && (Globals.Instance.NumPlayer > 1))
            {
                totalSeconds -= timeSeconds;

                if (totalSeconds <= 0)
                {
                    timeOver = true;
                }
                else
                {
                    ShowTime(totalSeconds);
                }
            }

            if (timeOver && !timeOverEnd)
            {
                overTime -= timeSeconds;

                if (overTime <= 0)
                {
                    if (Players[0].Score > Players[1].Score)
                    {
                        Globals.Instance.RoundScore[0]++;
                    }
                    else
                    {
                        Globals.Instance.RoundScore[1]++;
                    }
                    ItemManager.Instance.Action(EColorType.MSG, this, 5);
                    timeOverEnd = true;
                }
            }

            if (sdkCameraMan != null)
            {
                sdkCameraMan.frameRenderingQueued(new FrameEvent() { timeSinceLastFrame = timeSeconds });
            }

            ItemManager.Instance.Update(timeSeconds, this);

            if (leftBD)
            {
                timeElapse += timeSeconds;
                int addPower = (int)(timeSeconds * powerStep);

                if ((dirFlag > 0 && (power + addPower) >= maxPower) ||
                    (dirFlag < 0 && (power - addPower) <= 0))
                {
                    dirFlag = -dirFlag;
                }

                power += dirFlag * addPower;
                int powerSize = (int)(power * scalar);
                powerCover.SetSize(60, powerSize);
            }

            if (fire)
            {
                Vector3 curPos = cue.GetCurPos();
                if (curPos.x > 9)
                {
                    cue.SetCuePos(curPos - new Vector3(0.75f, 0, 0) * timeSeconds * 10);
                }
                else
                {
                    CurrentPlayer.SetCurScalar(CurrentPlayer.Scalar);
                    CurrentPlayer.SetCurHoldTurn(CurrentPlayer.IsHoldTurn);
                    Vector3 dirfromZ = cue.GetDirUnder(-Vector3.UNIT_X);
                    dirfromZ = dirfromZ * power;
                    whiteBall.AddForce(dirfromZ, ForceModes.Impulse);
                    cue.SetVisible(false);
                    cue.SetCuePos(new Vector3(cue.DistToTarget, 0, 0));
                    fire = false;
                    canHit = false;
                }
            }
            Vector3 ballPos = whiteBall.GetGlobalPosition();
            cue.SetTargetPos(ballPos);

            physxScene.FlushStream();
            physxScene.Simulate(timeSeconds);
            physxScene.FetchResults(SimulationStatuses.AllFinished, true);

            UpdateActorNodes((float)timeSeconds);
        }

        private void UpdateActorNodes(float deltaTime)
        {
            foreach (var actorNode in actorNodes)
            {
                actorNode.Update(deltaTime);
            }
        }

        public void SetGameOver(bool isGameOver)
        {
            this.isGameOver = isGameOver;
        }

        public void FreezeTime(bool isFreezeTime)
        {
            freezeTime = isFreezeTime;
        }

        public bool OnJointBreak(float BreakingImpulse, Joint Joint)
        {
            return true;
        }
    }
}
