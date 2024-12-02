import { useState } from "react";
import { authService } from "../../services/auth";
import { useNavigate } from "react-router";
import { AxiosError } from "axios";

export default function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const navigate = useNavigate();

  const handleLogin = async () => {
    try {
      await authService.login(email, password);
      navigate({
        pathname: "/",
        search: "?message=Logged in successfully&messageType=success",
      });
    } catch (error) {
      console.error(error);
      if (error instanceof AxiosError) {
        navigate({
          search: `?message=${error.response?.data}&messageType=danger`,
        });
      } else {
        navigate({
          search: "?message=Login failed&messageType=danger",
        });
      }
    }
  };

  return (
    <div>
      <h1 className="text-center">Log in</h1>
      <div className="row justify-content-center">
        <div className="col-md-4">
          <section>
            <form
              id="account"
              onSubmit={(e) => e.preventDefault()}
              className="text-center"
            >
              <h2>Use a local account to log in.</h2>
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
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                />
                <label htmlFor="email" className="form-label">
                  Email
                </label>
                <span className="text-danger"></span>
              </div>

              <div className="form-floating mb-3">
                <input
                  type="password"
                  className="form-control"
                  id="password"
                  autoComplete="current-password"
                  aria-required="true"
                  placeholder="password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                />
                <label htmlFor="password" className="form-label">
                  Password
                </label>
                <span className="text-danger"></span>
              </div>

              <div>
                <button
                  id="login-submit"
                  type="submit"
                  className="w-100 btn btn-lg btn-primary"
                  onClick={handleLogin}
                >
                  Log in
                </button>
              </div>

              <div>
                <p>
                  <a href="/register">Register as a new user</a>
                </p>
              </div>
            </form>
          </section>
        </div>
      </div>
    </div>
  );
}
