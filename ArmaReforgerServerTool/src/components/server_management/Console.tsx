import { Terminal } from "lucide-react";

export default function Console() {
  return (
    <div className="col-span-3 space-y-2">
      <div className="flex items-center gap-2 text-xs font-mono text-muted-foreground">
        <Terminal className="h-3 w-3 text-brand-orange" /> CONSOLE
      </div>
      <div className="console-screen flex-1 h-[600px] rounded-md overflow-y-auto border border-white/10 min-h-0 font-mono text-green-400">
        <p>[08:22:01] Initializing Game Server...</p>
        <p>[08:22:05] Loading Mods: @CBA_A3, @ACE...</p>
        <p>[08:22:10] Server identity verified.</p>
        <p className="text-brand-orange">[08:22:12] Mission: Combat_Op_Everon started.</p>
        <p className="animate-pulse">_</p>
      </div>
    </div>
  );
}