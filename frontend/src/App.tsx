import { BrowserRouter, Routes, Route } from "react-router";
import Layout from "./components/layouts/Layout";

import Login from "./components/auth/Login";
import Register from "./components/auth/Register";

import MyProducersPage from "./components/producers/MyProducers/Page";
import UpdateProducerPage from "./components/producers/UpdateProducer/Page";
import CreateProducerPage from "./components/producers/CreateProducer/Page";

import AllProductsPage from "./components/products/AllProducts/Page";
import MyProductsPage from "./components/products/MyProducts/Page";
import ProductDetailsPage from "./components/products/ProductDetails/Page";
import CreateProductPage from "./components/products/CreateProduct/Page";
import UpdateProductPage from "./components/products/UpdateProduct/Page";

export function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<Layout />}>
          {/* Product-related routes */}
          <Route index element={<AllProductsPage />} />
          <Route path="/products/my" element={<MyProductsPage />} />
          <Route path="/products/:productId" element={<ProductDetailsPage />} />
          <Route path="/products/create" element={<CreateProductPage />} />
          <Route
            path="/products/:productId/update"
            element={<UpdateProductPage />}
          />

          {/* Producer-related routes */}
          <Route path="/producers/my" element={<MyProducersPage />} />
          <Route path="/producers/create" element={<CreateProducerPage />} />
          <Route
            path="/producers/:producerId/update"
            element={<UpdateProducerPage />}
          />

          {/* Auth-related routes */}
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}
