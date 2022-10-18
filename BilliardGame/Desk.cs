using Mogre;
using Mogre.PhysX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilliardGame
{
    public class Desk
    {
        private string meshName;
        private Vector3 originalPose;
        private List<ActorSceneNode> deskParts;
        public List<ActorSceneNode> DeskParts
        {
            get { return deskParts; }
        }

        public Desk(string meshName, Vector3 originalPose)
        {
            this.meshName = meshName;
            this.originalPose = originalPose;

            deskParts = new List<ActorSceneNode>();
        }

        public void Initialise(Scene physxScene, SceneManager sceneManager)
        {
            Entity ent;
            SceneNode entNode = createEntitySceneNode(sceneManager, "DECK_" + Guid.NewGuid().ToString(), meshName, out ent);
            Actor entActor = createEntityActor(ent, physxScene);
            ActorSceneNode entActorNode = new ActorSceneNode(entActor, entNode);
            deskParts.Add(entActorNode);

            entNode = createEntitySceneNode(sceneManager, "DECK_SIZE_" + Guid.NewGuid().ToString(), meshName + "_size", out ent);
            entActor = createEntityActor(ent, physxScene);
            entActorNode = new ActorSceneNode(entActor, entNode);
            deskParts.Add(entActorNode);

            entNode = createEntitySceneNode(sceneManager, "DECK_HOLELT_" + Guid.NewGuid().ToString(), meshName + "_holelt", out ent);
            entActor = createEntityActor(ent, physxScene);
            entActorNode = new ActorSceneNode(entActor, entNode);
            deskParts.Add(entActorNode);

            entNode = createEntitySceneNode(sceneManager, "DECK_HOLERT_" + Guid.NewGuid().ToString(), meshName + "_holert", out ent);
            entActor = createEntityActor(ent, physxScene);
            entActorNode = new ActorSceneNode(entActor, entNode);
            deskParts.Add(entActorNode);

            entNode = createEntitySceneNode(sceneManager, "DECK_HOLELM_" + Guid.NewGuid().ToString(), meshName + "_holelm", out ent);
            entActor = createEntityActor(ent, physxScene);
            entActorNode = new ActorSceneNode(entActor, entNode);
            deskParts.Add(entActorNode);

            entNode = createEntitySceneNode(sceneManager, "DECK_HOLERM_" + Guid.NewGuid().ToString(), meshName + "_holerm", out ent);
            entActor = createEntityActor(ent, physxScene);
            entActorNode = new ActorSceneNode(entActor, entNode);
            deskParts.Add(entActorNode);

            entNode = createEntitySceneNode(sceneManager, "DECK_HOLELB_" + Guid.NewGuid().ToString(), meshName + "_holelb", out ent);
            entActor = createEntityActor(ent, physxScene);
            entActorNode = new ActorSceneNode(entActor, entNode);
            deskParts.Add(entActorNode);

            entNode = createEntitySceneNode(sceneManager, "DECK_HOLERB_" + Guid.NewGuid().ToString(), meshName + "_holerb", out ent);
            entActor = createEntityActor(ent, physxScene);
            entActorNode = new ActorSceneNode(entActor, entNode);
            deskParts.Add(entActorNode);
        }

        private SceneNode createEntitySceneNode(SceneManager sceneManager, string name, string meshName, out Entity ent)
        {
            ent = sceneManager.CreateEntity(name, meshName + ".mesh");
            SceneNode entSceneNode = sceneManager.RootSceneNode.CreateChildSceneNode();
            entSceneNode.AttachObject(ent);

            return entSceneNode;
        }

        private Actor createEntityActor(Entity ent, Scene physxScene)
        {
            ActorDesc actorDesc = new ActorDesc();
            actorDesc.Body = null;
            TriangleMeshShapeDesc triangleMeshShapeDesc = physxScene.Physics.CreateTriangleMesh(new StaticMeshData(ent.GetMesh()));
            actorDesc.Shapes.Add(triangleMeshShapeDesc);
            Actor actor = physxScene.CreateActor(actorDesc);
            return actor;
        }
    }
}
