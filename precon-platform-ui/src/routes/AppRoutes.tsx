import { Routes, Route } from "react-router-dom";
import AppShell from "../components/layout/AppShell";
import DashboardPage from "../components/dashboard/DashboardPage";
import WeatherAnalyticsPage from "../pages/WeatherAnalyticsPage";
import DesignPage from "../pages/DesignPage";
import DocumentPage from "../pages/DocumentPage";
import EstimatingPage from "../pages/EstimatingPage";

export default function AppRoutes(){
  return (
    <Routes>
      <Route element={<AppShell />}>
        <Route index element={<DashboardPage />} />
        <Route path="weather" element={<WeatherAnalyticsPage />} />
        <Route path="design" element={<DesignPage />} />
        <Route path="document" element={<DocumentPage />} />
        <Route path="estimating" element={<EstimatingPage />} />
      </Route>
    </Routes>
  );
}
