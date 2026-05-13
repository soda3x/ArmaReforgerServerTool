import { ServerParameterTextArea } from "@/components/server_configuration/controls";
import { cn } from "@/lib/utils";
import { Braces } from "lucide-react";

type MissionHeaderProps = {
  className?: string
}
export default function ServerParamsMissionHeader({ className }: MissionHeaderProps) {
  return (
    <div className={cn("flex flex-col h-full w-full gap-2", className)}>
      <div className="flex items-center gap-2 text-xs font-mono text-muted-foreground">
        <Braces className="h-3 w-3 text-brand-orange" /> MISSION_HEADER
      </div>
      <div className="flex flex-col flex-1 min-h-0 mt-1">
        <ServerParameterTextArea id="mission-header" placeholder="Edit this to override the mission header for your scenario. You can find details on what to put here on the Arma Workshop." />
      </div>
    </div>
  );
}