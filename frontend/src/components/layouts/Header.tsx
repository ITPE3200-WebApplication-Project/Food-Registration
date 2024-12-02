import { useLocation } from "react-router";
import { authService } from "../../services/auth";
import AuthHeader from "../auth/AuthHeader";
import { useEffect } from "react";

export default function Header() {
  let user = authService.getUser();

  const location = useLocation();

  // Update user state when path changes
  useEffect(() => {
    user = authService.getUser();
  }, [location.pathname]);

  return (
    <header>
      <nav className="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
        <div className="container-fluid">
          <a className="navbar-brand" href="/">
            <img
              src="/images/logo.png"
              alt="Logo"
              className="logo"
              style={{ height: "100px" }}
            />
          </a>

          <button
            className="navbar-toggler"
            type="button"
            data-bs-toggle="collapse"
            data-bs-target=".navbar-collapse"
            aria-controls="navbarSupportedContent"
            aria-expanded="false"
            aria-label="Toggle navigation"
          >
            <span className="navbar-toggler-icon"></span>
          </button>
          <div className="navbar-collapse collapse d-sm-inline-flex justify-content-between">
            <ul className="navbar-nav mx-auto">
              <li className="nav-item">
                <a className="nav-link text-dark" href="/">
                  All Products
                </a>
              </li>
              {user.loggedIn && (
                <>
                  <li className="nav-item">
                    <a className="nav-link text-dark" href="/products/create">
                      New Product
                    </a>
                  </li>
                  <li className="nav-item">
                    <a className="nav-link text-dark" href="/products/my">
                      My Products
                    </a>
                  </li>
                  <li className="nav-item">
                    <a className="nav-link text-dark" href="/producers/my">
                      My Producers
                    </a>
                  </li>
                </>
              )}
            </ul>
            <AuthHeader />
          </div>
        </div>
      </nav>
    </header>
  );
}
