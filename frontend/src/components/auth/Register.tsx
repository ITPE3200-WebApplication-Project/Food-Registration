import { useState } from "react";
import { IRegisterDTO } from "../../types/dtos";
import { authService } from "../../services/auth";
import { useNavigate } from "react-router";
import { AxiosError } from "axios";

export default function Register() {
  const navigate = useNavigate();
  const [error, setError] = useState("");
  const [user, setUser] = useState<IRegisterDTO>({
    email: "",
    password: "",
    confirmPassword: "",
  });

  // handle register
  const handleRegister = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setError("");

    try {
      if (user.password !== user.confirmPassword) {
        setError("Passwords do not match");
        return;
      }

      await authService.register(
        user.email,
        user.password,
        user.confirmPassword
      );

      navigate({
        pathname: "/",
        search: "?message=Registered successfully&messageType=success",
      });
    } catch (error) {
      console.error(error);
      if (error instanceof AxiosError) {
        navigate({
          search: `?message=${error.response?.data}&messageType=danger`,
        });
      } else {
        navigate({
          search: "?message=Registration failed&messageType=danger",
        });
      }
    }
  };

  return (
    <div>
      <h1 className="text-center">Register</h1>

      <div className="row justify-content-center text-center">
        <div className="col-md-4">
          <form
            id="registerForm"
            onSubmit={handleRegister}
            className="text-center"
          >
            <h2 className="text-center">Create a new account.</h2>
            <hr />
            {error && (
              <div className="text-danger" role="alert">
                {error}
              </div>
            )}

            <div className="form-floating mb-3">
              <input
                type="email"
                className="form-control"
                id="email"
                autoComplete="username"
                aria-required="true"
                placeholder="name@example.com"
                value={user.email}
                onChange={(e) => setUser({ ...user, email: e.target.value })}
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
                value={user.password}
                onChange={(e) => setUser({ ...user, password: e.target.value })}
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
                value={user.confirmPassword}
                onChange={(e) => {
                  // Check if passwords match
                  if (user.password !== e.target.value) {
                    setError("Passwords do not match");
                  }
                  // update user state
                  setUser({ ...user, confirmPassword: e.target.value });
                }}
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
