import { BrowserRouter, Routes, Route } from "react-router";
import { ProductList } from "./components/products/ProductList";
import Login from "./components/auth/Login";
import Layout from "./components/layouts/Layout";
import Register from "./components/auth/Register";
// import Table from "./components/products/Table";
import Create from "./components/products/pages/Create";
import { UserProvider } from "./contexts/UserContext";
import MyProducts from "./components/products/pages/MyProducts";
import MyProducers from "./components/producers/pages/MyProducers";

export function App() {
  return (
    <UserProvider>
      <BrowserRouter>
        <Routes>
          <Route element={<Layout />}>
            <Route index element={<ProductList />} />
            <Route path="/products/my" element={<MyProducts />} />
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            {/* <Route path="/products/table" element={<Table />} /> */}
            <Route path="/products/create" element={<Create />} />
            <Route path="/producers/my" element={<MyProducers />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </UserProvider>
  );
}
