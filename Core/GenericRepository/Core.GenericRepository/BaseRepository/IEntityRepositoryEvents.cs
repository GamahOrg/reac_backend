using static Core.GenericRepository.Shared.RepositoryDelegates;

namespace Core.GenericRepository.BaseRepository
{
    public interface IEntityRepositoryEvents
    {
        public event BeforeUpdateDelegate BeforeUpdateEvent;
        public event AfterUpdateDelegate AfterUpdateEvent;
        public event BeforeAddDelegate BeforeAddEvent;
        public event AfterAddDelegate AfterAddEvent;
        public event BeforeDeleteDelegate BeforeDeleteEvent;
        public event AfterDeleteDelegate AfterDeleteEvent;
        public event AfterSaveDelegate AfterSaveEvent;
        public event BeforeSaveDelegate BeforeSaveEvent;
    }
}
