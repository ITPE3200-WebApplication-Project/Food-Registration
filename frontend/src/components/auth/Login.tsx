export default function Login() {
  return (
    <ul className="navbar-nav">
      <li className="nav-item">
        <a
          id="manage"
          className="nav-link text-dark"
          asp-area="Identity"
          asp-page="/Account/Manage/Index"
          title="Manage"
        >
          Hello @UserManager.GetUserName(User)!
        </a>
      </li>
      <li className="nav-item">
        <form id="logoutForm" className="form-inline">
          <button
            id="logout"
            type="submit"
            className="nav-link btn btn-link text-dark border-0"
          >
            Logout
          </button>
        </form>
      </li>
      <li className="nav-item">
        <a
          className="nav-link text-dark"
          id="register"
          asp-area="Identity"
          asp-page="/Account/Register"
        >
          Register
        </a>
      </li>
      <li className="nav-item">
        <a
          className="nav-link text-dark"
          id="login"
          asp-area="Identity"
          asp-page="/Account/Login"
        >
          Login
        </a>
      </li>
    </ul>
  );
}
