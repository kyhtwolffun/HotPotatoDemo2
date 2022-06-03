namespace Quantum
{
    public unsafe class MovementSystem : SystemMainThreadFilter<MovementSystem.Filter>, ISignalOnPlayerDataSet
    {

        public struct Filter
        {
            public EntityRef Entity;
            public CharacterController3D* KCC;
            public Transform3D* Transform;
            public PlayerLink* Link;
        }
        public override void Update(Frame f, ref Filter filter)
        {
            var input = f.GetPlayerInput(filter.Link->Player);

            if (input->Jump.WasPressed)
            {
                filter.KCC->Jump(f);
            }

            // speed hack protection
            if (input->Direction.Magnitude > 1)
            {
                input->Direction = input->Direction.Normalized;
            }

            filter.KCC->Move(f, filter.Entity, input->Direction.XOY);

            if (input->Direction != default)
            {
                filter.Transform->Rotation = Photon.Deterministic.FPQuaternion.LookRotation(input->Direction.XOY);
            }
        }

        public void OnPlayerDataSet(Frame f, PlayerRef player)
        {
            var data = f.GetPlayerData(player);
            var prototype = f.FindAsset<EntityPrototype>(data.CharacterPrototype.Id);
            var e = f.Create(prototype);

            if (f.Unsafe.TryGetPointer<Transform3D>(e, out var tt))
            {
                tt->Position.X = player._index;
            }

            if (f.Unsafe.TryGetPointer<PlayerLink>(e, out var pl))
            {
                pl->Player = player;
            }
        }
    }
}
