using GreenBill.MVVM.Model;
using System;

namespace GreenBill.Services
{
    public interface IUserSessionService
    {
        User CurrentUser { get; }
        bool IsUserLoggedIn { get; }
        void SetCurrentUser(User user);
        void ClearSession();
        event EventHandler<User> UserLoggedIn;
        event EventHandler UserLoggedOut;
    }

    public class UserSessionService : IUserSessionService
    {
        private static UserSessionService _instance;
        private static readonly object _lock = new object();

        private User _currentUser;

        public static UserSessionService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new UserSessionService();
                    }
                }
                return _instance;
            }
        }

        private UserSessionService() { }

        public User CurrentUser => _currentUser;

        public bool IsUserLoggedIn => _currentUser != null;

        public event EventHandler<User> UserLoggedIn;
        public event EventHandler UserLoggedOut;

        public void SetCurrentUser(User user)
        {
            _currentUser = user;
            UserLoggedIn?.Invoke(this, user);
        }

        public void ClearSession()
        {
            _currentUser = null;
            UserLoggedOut?.Invoke(this, EventArgs.Empty);
        }
    }
}
