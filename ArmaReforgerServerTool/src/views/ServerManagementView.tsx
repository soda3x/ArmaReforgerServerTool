import { ServerParamsMissionHeader } from "@/components/server_configuration/reforger";
import { Console, ModManager, PerfMetrics, QuickActions, ServerConfig, ServerInfo } from "@/components/server_management";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { ChevronRight, Play, Square } from "lucide-react";

export default function ServerManagementView({ serverId }: { serverId: string }) {
  return (
    <div className="flex flex-col h-full space-y-6 animate-in slide-in-from-right-4 duration-300 pb-10">
      <div className="flex justify-between items-center">
        <div>
          <h2 className="text-2xl font-bold text-white">{serverId.toUpperCase()}</h2>
          <div className="flex gap-2">
            <Badge className="bg-green-400 text-black">ONLINE</Badge>
            <Badge className="bg-gray-600">Uptime: 14h 22m</Badge>
            <Badge>Arma Reforger</Badge>
          </div>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" size="sm" className="border-red-500/50 text-red-500 hover:bg-red-500/10">
            <Square className="h-4 w-4 mr-2" /> Stop
          </Button>
          <Button size="sm" className="bg-brand-orange hover:bg-brand-orange/80">
            <Play className="h-4 w-4 mr-2" /> Restart
          </Button>
        </div>
      </div>
      <Tabs defaultValue="manage" className="flex-1 flex flex-col min-h-0">
        <TabsList className="w-fit shrink-0 bg-white-800">
          <TabsTrigger value="configure">Configure <ChevronRight /></TabsTrigger>
          <TabsTrigger value="manage-mods">Manage Mods <ChevronRight /></TabsTrigger>
          <TabsTrigger value="choose-scenario">Choose Scenario <ChevronRight /></TabsTrigger>
          <TabsTrigger value="edit-mission-header">Edit Mission Header <ChevronRight /></TabsTrigger>
          <TabsTrigger value="advanced">Advanced <ChevronRight /></TabsTrigger>
          <TabsTrigger value="manage">Monitor</TabsTrigger>
        </TabsList>
        <TabsContent value="manage" className="flex-1 min-h-0 m-0 data-[state=active]:flex flex-col pt-4">
          <div className="grid grid-cols-4 gap-6 flex-1 min-h-0 w-full">
            {/* Main Panel */}
            <div className="space-y-2 col-span-3 flex flex-col h-[600px]">
              <Console />
            </div>

            {/* Right Sidebar */}
            <div className="flex flex-col space-y-4 flex-1 min-h-0">
              <PerfMetrics />
              <ServerInfo />
              <QuickActions />
            </div>
          </div>
        </TabsContent>
        <TabsContent value="configure" className="flex-1 min-h-0 pt-4">
          <ServerConfig />
        </TabsContent>
        <TabsContent value="edit-mission-header" className="flex-1 min-h-0 pt-4">
          <ServerParamsMissionHeader />
        </TabsContent>
        <TabsContent value="manage-mods" className="flex-1 min-h-0 pt-4">
          <ModManager />
        </TabsContent>
      </Tabs>
    </div>
  );
}