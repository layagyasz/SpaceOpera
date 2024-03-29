﻿using Cardamom.Trackers;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Economics.Projects
{
    public abstract class BaseProject : IProject
    {
        public abstract object Key { get; }
        public abstract string Name { get; }
        public abstract Faction Faction { get; }
        public abstract Pool Progress { get; }
        public ProjectStatus Status { get; protected set; } = ProjectStatus.InProgress;

        public void Tick()
        {
            TickImpl();
            if (Progress.IsFull())
            {
                Status = ProjectStatus.Done;
            }
        }

        public void Cancel()
        {
            CancelImpl();
            Status = ProjectStatus.Cancelled;
        }

        public abstract void Setup();
        public abstract void Finish(World world);
        protected abstract void TickImpl();
        protected abstract void CancelImpl();
    }
}
