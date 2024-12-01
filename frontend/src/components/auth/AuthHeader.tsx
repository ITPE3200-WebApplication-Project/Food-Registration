import { authService } from "../../services/auth";

export default function AuthHeader() {
  const user = authService.getUser();

  return (
    <div className="d-flex">
      <ul className="navbar-nav">
        {user.loggedIn && (
          <>
            <li className="nav-item nav-link text-dark">Hello {user.email}</li>
            <li className="nav-item">
              <form id="logoutForm" className="form-inline">
                <button
                  id="logout"
                  onClick={authService.logout}
                  className="nav-link btn btn-link text-dark border-0"
                >
                  Logout
                </button>
              </form>
            </li>
          </>
        )}
        {!user.loggedIn && (
          <>
            <li className="nav-item">
              <a className="nav-link text-dark" id="register" href="/register">
                Register
              </a>
            </li>
            <li className="nav-item">
              <a className="nav-link text-dark" id="login" href="/login">
                Login
              </a>
            </li>
          </>
        )}
      </ul>
    </div>
  );
}
