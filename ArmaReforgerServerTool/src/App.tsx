import "@/App.css";
import { useState } from "react";
import { SidebarProvider, SidebarTrigger } from "@/components/ui/sidebar";
import { AppSidebar } from "@/components/app/AppSidebar";
import { WindowControls } from "@/components/app/WindowControls";
import { DashboardView } from "@/views/DashboardView";
import ServerManagementView from "@/views/ServerManagementView";
import { Label } from "./components/ui/label";
import { Button } from "./components/ui/button";
import { ChevronLeft } from "lucide-react";

type PageId = "dashboard" | "reforger" | "arma3" | "settings";

function App() {
  const [activePage, setActivePage] = useState<PageId>("dashboard");

  // A helper to render the correct component
  const renderContent = () => {
    if (activePage.startsWith("manage-")) {
      const serverId = activePage.replace("manage-", "");
      return <ServerManagementView serverId={serverId} />;
    }

    switch (activePage) {
      case "dashboard": return <DashboardView onSelectServer={(id) => setActivePage(`manage-${id}` as PageId)} />;
      // case "settings": return <SettingsView />;
      default: return <DashboardView onSelectServer={(id) => setActivePage(`manage-${id}` as PageId)} />;
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
          <main className="flex-1 flex flex-col min-w-0 p-10 overflow-y-auto">
            {activePage !== "dashboard" &&
              <Button
                variant="ghost"
                className="w-30 text-gray-200"
                onClick={() => setActivePage("dashboard")}
              >
                <ChevronLeft /> Dashboard
              </Button>
            }
            {renderContent()}
          </main>
        </div>
      </SidebarProvider>
    </div>
  );
}

export default App;