import "./App.css";
import { useState } from "react";
import { SidebarProvider, SidebarTrigger } from "@/components/ui/sidebar";
import { AppSidebar } from "./components/AppSidebar";
import { WindowControls } from "./components/WindowControls";

type PageId = "dashboard" | "reforger" | "arma3" | "settings";

function App() {
const [activePage, setActivePage] = useState<PageId>("dashboard");

  // A helper to render the correct component
  const renderContent = () => {
    switch (activePage) {
      case "dashboard": return <DashboardView />;
      case "settings": return <SettingsView />;
      default: return <DashboardView />;
    }
  };

  return (
    <div className="flex flex-col h-screen overflow-hidden bg-background">
      <WindowControls />
      <SidebarProvider>
        <div className="flex h-full w-full overflow-hidden">
          <AppSidebar />
          <div className="md:hidden">
            <SidebarTrigger />
          </div>
          <main className="flex-1 flex flex-col min-w-0">
          </main>
        </div>
      </SidebarProvider>
    </div>
  );
}

export default App;