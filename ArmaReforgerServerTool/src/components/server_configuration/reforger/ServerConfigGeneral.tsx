import { Xbox } from "@/components/icons/Xbox";
import { PlayStation } from "@/components/icons/PlayStation";
import { ServerParameterBoolean, ServerParameterInput } from "@/components/server_configuration/controls";
import { Label } from "@/components/ui/label";

export default function ServerParamsGeneral() {
  return (
    <>
      <Label className="pb-2 text-lg">General</Label>
      <div className="flex flex-col gap-4">
        <ServerParameterInput id="server-name" label="Server Name" placeholder="My Longbow Arma Server" defaultValue="My Longbow Arma Server" required />
        <div className="flex items-center justify-between gap-2">
          <ServerParameterInput id="server-pw" label="Server Password" />
          <ServerParameterInput id="admin-pw" label="Admin Password" />
        </div>
        <ServerParameterInput id="max-players" label="Max Players" type="number" />
        <div className="flex items-center justify-between gap-2">
          <ServerParameterBoolean id="cross-platform" label="Cross Platform" hint={<>Enable to play with <Xbox /> and <PlayStation /> players</>} />
          <ServerParameterBoolean id="server-visible" label="Server Visible" startState={true} hint="Required for discovery in Server Browser" />
        </div>
        <div className="flex items-center justify-between gap-2">
          <ServerParameterBoolean id="battleye" label="BattlEye Anti-Cheat" startState={true} />
          <ServerParameterBoolean id="disable-third-person" label="Disable Third Person" />
        </div>
        <div className="flex items-center justify-between gap-2">
          <ServerParameterBoolean id="fast-validation" label="Fast Validation" startState={true} hint="Always enable if server is public" />
          <ServerParameterBoolean id="use-experimental" label="Experimental Server" />
        </div>
      </div>
    </>
  );
}