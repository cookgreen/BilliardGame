using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mogre;
using Mogre.PhysX;

using org.ogre.framework;

namespace BilliardGame
{
    public class Ball
    {
        //private string type;
        private EColorType color;
        //private Mogre.Material mat;
        private Vector3 originalPos;

        public ActorSceneNode BallActorSceneNode { get; set; }
        public EColorType ColorType { get { return color; } }

        public Ball(EColorType color, Vector3 initPos)
        {
            this.color = color;
            originalPos = initPos;
        }

        public void Initialise(Scene physxScene, SceneManager sceneManager)
        {
            BodyDesc ballBodyDesc = new BodyDesc();
            ballBodyDesc.Mass = 40;
            ballBodyDesc.LinearDamping = 0.1f;
            ballBodyDesc.AngularDamping = 1;

            SphereShapeDesc sphereShapeDesc = new SphereShapeDesc();
            sphereShapeDesc.Radius = 0.5f;

            ActorDesc ballActorDesc = new ActorDesc();
            ballActorDesc.Body = ballBodyDesc;
            ballActorDesc.Shapes.Add(sphereShapeDesc);

            Entity ballEnt = sceneManager.CreateEntity(
                "ball_" + color.ToString() + "_" + Guid.NewGuid().ToString(),
                "sphere_t.mesh");
            ballEnt.SetMaterialName("ball_" + ((int)color).ToString());
            SceneNode ballSceneNode = sceneManager.RootSceneNode.CreateChildSceneNode();
            ballSceneNode.Position = originalPos;
            ballSceneNode.AttachObject(ballEnt);

            Actor ballActor = physxScene.CreateActor(ballActorDesc);
            ballActor.GlobalPosition = originalPos;
            BallActorSceneNode = new ActorSceneNode(ballActor, ballSceneNode);
        }

        public Vector3 GetGlobalPosition()
        {
            Vector3 vect = BallActorSceneNode.Actor.GlobalPosition;
            return vect;
        }

        public void SetGlobalPosition(Vector3 newPos)
        {
            BallActorSceneNode.Actor.GlobalPosition = newPos;
            BallActorSceneNode.SceneNode.Position = newPos;
        }

        public void AddForce(Vector3 dir, ForceModes forceModes)
        {
            BallActorSceneNode.Actor.AddForce(dir, forceModes);
        }

        public static int GetScoreOf(EColorType color)
        {
            switch(color)
            {
                case EColorType.WHITE:
                    return -1;
                case EColorType.RED:
                    return 1;
                case EColorType.GREEN:
                case EColorType.BLUE:
                case EColorType.BROWN:
                case EColorType.BLACK:
                    return 2;
                default:
                    return 0;
            }
        }
    }
}
