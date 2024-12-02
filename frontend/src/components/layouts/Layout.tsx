import { Outlet } from "react-router";
import Header from "./Header";
import Footer from "./Footer";
import MessageDialog from "../shared/MessageDialog";

export default function Layout() {
  return (
    <div>
      <Header />
      <main className="container pb-3">
        <MessageDialog />
        <Outlet />
      </main>
      <Footer />
    </div>
  );
}
