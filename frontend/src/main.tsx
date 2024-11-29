import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import { BrowserRouter, Route, Routes } from "react-router";
import AllProducts from "./components/products/AllProducts";
import "./scss/styles.scss";
// Import all of Bootstrap's JS
import Layout from "./components/layouts/Layout";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <BrowserRouter>
      <Routes>
        <Route element={<Layout />}>
          <Route index element={<AllProducts />} />
        </Route>
      </Routes>
    </BrowserRouter>
  </StrictMode>
);
