namespace Quantum
{
    public unsafe class MovementSystem : SystemMainThreadFilter<MovementSystem.Filter>
    {

        public struct Filter
        {
            public EntityRef Entity;
            public MovementComp* movementComp;
            public Transform3D* transform;
            public PhysicsBody3D* body;
            public PlayerLink* Link;
        }
        public override void Update(Frame f, ref Filter filter)
        {
            var input = f.GetPlayerInput(filter.Link->Player);

            if (input->Jump.WasPressed)
            {
                filter.body->Velocity.Y = 1 * filter.movementComp->jumpForce;
            }

            if (input->Direction.Magnitude > 0)
            {
                // speed hack protection
                if (input->Direction.Magnitude > 1)
                {
                    input->Direction = input->Direction.Normalized;
                }

                filter.body->Velocity.X = input->Direction.X * filter.movementComp->speed;
                filter.body->Velocity.Z = input->Direction.Y * filter.movementComp->speed;
            }

            if (input->Direction != default)
            {
                filter.transform->Rotation = Photon.Deterministic.FPQuaternion.LookRotation(input->Direction.XOY);
            }
        }
    }
}