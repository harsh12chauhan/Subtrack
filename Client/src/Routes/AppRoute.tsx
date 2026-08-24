import { Route } from "react-router-dom";
import DashboardLayout from "../Layouts/DashboardLayout";
import Dashboard from "../Pages/Dashboard";
import Subscription from "../Components/Subscription";
import Payment from "../Components/Payment";

const AppRoute = () => {
  return (
    <Route element={<DashboardLayout />}>
      <Route path="/dashboard" element={<Dashboard />} />
      <Route path="/subscriptions" element={<Subscription />} />
      <Route path="/payments" element={<Payment />} />
    </Route>
  );
};

export default AppRoute;
