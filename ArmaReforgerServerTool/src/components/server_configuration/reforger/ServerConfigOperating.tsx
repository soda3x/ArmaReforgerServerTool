import { ServerParameterBoolean, ServerParameterInput } from "@/components/server_configuration/controls";
import { Label } from "@/components/ui/label";

export default function ServerParamsOperating() {
  return (
    <>
      <Label className="pb-2 text-lg">Operating</Label>
      <div className="flex flex-col gap-4">
        <div className="flex items-center justify-between gap-2">
          <ServerParameterBoolean id="lobby-player-synchronise" label="Lobby Player Synchronise" startState={true} />
          <ServerParameterBoolean id="disable-crash-reporter" label="Disable Crash Reporter" />
        </div>
        <div className="flex items-center justify-between gap-2">
          <ServerParameterBoolean id="disable-navmesh-streaming" label="Disable Navmesh Streaming" hint="Work in progress" />
          <ServerParameterBoolean id="disable-server-shutdown" label="Disable Server Shutdown" />
        </div>
        <ServerParameterBoolean id="disable-ai" label="Disable AI" />
        <div className="flex items-center justify-between gap-2">
          <ServerParameterInput id="player-save-time" label="Player Save Time" defaultValue={120} type="number" />
          <ServerParameterInput id="ai-limit" label="AI Limit" defaultValue={-1} type="number" hint="-1 indicates no limit" />
        </div>
        <div className="flex items-center justify-between gap-2">
          <ServerParameterInput id="slot-reservation-timeout" label="Slot Reservation Timeout" minValue={5} maxValue={300} defaultValue={60} type="number" />
          <ServerParameterInput id="join-queue-max-size" label="Join Queue Max Size" minValue={0} maxValue={50} defaultValue={0} type="number" hint="0 indicates disabled" />
        </div>
      </div>
    </>
  );
}