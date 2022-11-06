using Mogre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilliardGame
{
    public class Cue
    {
        private string meshName;
        private float distToTarget;
        private SceneNode fakePivot;
        private SceneNode cueNode;

        public float DistToTarget { get { return distToTarget; } }
        public SceneNode FakePivot { get { return fakePivot; } }
        public SceneNode CueNode { get { return cueNode; } }

        public Cue(string meshName)
        {
            this.meshName = meshName;
            distToTarget = 10;
        }

        public void Initialise(SceneManager sceneManager)
        {
            Entity cueEnt = sceneManager.CreateEntity("bar", meshName + ".mesh");
            fakePivot = sceneManager.RootSceneNode.CreateChildSceneNode("Pivot");
            cueNode = fakePivot.CreateChildSceneNode("CurNode");
            SceneNode entNode = cueNode.CreateChildSceneNode();
            entNode.AttachObject(cueEnt);
            entNode.Rotate(Vector3.UNIT_Z, new Degree(90), Node.TransformSpace.TS_WORLD);

            cueNode.SetPosition(distToTarget, 0, 0);
        }

        public void SetTargetPos(Vector3 pos)
        {
            fakePivot.SetPosition(pos.x, pos.y, pos.z);
        }

        public void Rotate(Vector3 axis, Degree degree)
        {
            fakePivot.Rotate(axis, degree);
        }

        public Vector3 GetDirUnder(Vector3 axis)
        {
            Vector3 dir = cueNode._getDerivedOrientation() * axis;
            dir.Normalise();
            return dir;
        }

        public void SetVisible(bool visible)
        {
            cueNode.SetVisible(visible);
        }

        public void SetCuePos(Vector3 newPos)
        {
            cueNode.SetPosition(newPos.x, newPos.y, newPos.z);
        }

        public Vector3 GetCurPos()
        {
            return cueNode.Position;
        }
    }
}
