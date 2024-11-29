import Login from "../auth/Login";

export default function Header() {
  return (
    <header>
      <nav className="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
        <div className="container-fluid">
          <a className="navbar-brand" href="/">
            <img
              src="~/images/logo.png"
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
                <a
                  className="nav-link text-dark"
                  asp-area=""
                  asp-controller="Product"
                  asp-action="AllProducts"
                >
                  All Products
                </a>
              </li>
              <li className="nav-item">
                <a
                  className="nav-link text-dark"
                  asp-area=""
                  asp-controller="Product"
                  asp-action="NewProduct"
                >
                  New Product
                </a>
              </li>
              <li className="nav-item">
                <a
                  className="nav-link text-dark"
                  asp-area=""
                  asp-controller="Product"
                  asp-action="Table"
                >
                  My Products
                </a>
              </li>
              <li className="nav-item">
                <a
                  className="nav-link text-dark"
                  asp-controller="Producer"
                  asp-action="Table"
                >
                  My Producers
                </a>
              </li>
            </ul>
            <div className="d-flex">
              <Login />
            </div>
          </div>
        </div>
      </nav>
    </header>
  );
}
