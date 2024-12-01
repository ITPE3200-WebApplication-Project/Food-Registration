import { Outlet } from "react-router";
import Header from "./Header";
import Footer from "./Footer";

export default function Layout() {
  return (
    <div>
      <Header />
      <main className="container pb-3">
        <Outlet />
      </main>
      <Footer />
    </div>
  );
}
