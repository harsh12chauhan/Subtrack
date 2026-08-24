import { Navigate, Outlet } from "react-router-dom";
import { isAuthenticated } from "../Utils/Auth";
import Sidebar from "../Components/Sidebar";

const DashboardLayout = () => {
  if (!isAuthenticated()) {
    return <Navigate to="/" replace />;
  }

  return (
    <div className="flex">
      <Sidebar />

      <main className="flex-1 p-6 bg-slate-100 min-h-screen">
        <Outlet />
      </main>
    </div>
  );
};

export default DashboardLayout;
