export default function Register() {
  return (
    <div>
      <h1 className="text-center">Register</h1>

      <div className="row justify-content-center text-center">
        <div className="col-md-4">
          <form id="registerForm" method="post" className="text-center">
            <h2 className="text-center">Create a new account.</h2>
            <hr />
            <div className="text-danger" role="alert"></div>

            <div className="form-floating mb-3">
              <input
                type="email"
                className="form-control"
                id="email"
                autoComplete="username"
                aria-required="true"
                placeholder="name@example.com"
              />
              <label htmlFor="email">Email</label>
              <span className="text-danger"></span>
            </div>

            <div className="form-floating mb-3">
              <input
                type="password"
                className="form-control"
                id="password"
                autoComplete="new-password"
                aria-required="true"
                placeholder="password"
              />
              <label htmlFor="password">Password</label>
              <span className="text-danger"></span>
            </div>

            <div className="form-floating mb-3">
              <input
                type="password"
                className="form-control"
                id="confirmPassword"
                autoComplete="new-password"
                aria-required="true"
                placeholder="password"
              />
              <label htmlFor="confirmPassword">Confirm Password</label>
              <span className="text-danger"></span>
            </div>

            <button
              id="registerSubmit"
              type="submit"
              className="w-100 btn btn-lg btn-primary"
            >
              Register
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}
