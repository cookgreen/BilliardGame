using Mogre;
using Mogre.PhysX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilliardGame
{
    public class ActorSceneNode
    {
        private Actor actor;
        private SceneNode sceneNode;

        public Actor Actor
        {
            get { return actor; }
        }

        public SceneNode SceneNode
        {
            get { return sceneNode; }
        }

        public ActorSceneNode(Actor actor, SceneNode sceneNode)
        {
            this.actor = actor;
            this.sceneNode = sceneNode;
        }   

        public void Update(float timeSinceLastFrame)
        {
            if (actor != null)
            {
                if (!actor.IsSleeping)
                {
                    sceneNode.Position = actor.GlobalPosition;
                    sceneNode.Orientation = actor.GlobalOrientationQuaternion;
                }
            }
        }
    }
}
