import { getCurrentWindow } from "@tauri-apps/api/window";
import { X, Minus, Square } from "lucide-react";
import { Button } from "@/components/ui/button";

export function WindowControls() {
  const appWindow = getCurrentWindow();

  return (
    <div
      data-tauri-drag-region
      className="flex items-center justify-between h-10 bg-sidebar border-b border-sidebar-border select-none"
    >
      <div className="flex items-center px-5 gap-3 pointer-events-none">
        <div className="relative w-5 h-3 flex items-center justify-center">
          <div
            className="w-full h-full bg-white"
            style={{
              maskImage: 'url(/arma.svg)',
              WebkitMaskImage: 'url(/arma.svg)',
              maskRepeat: 'no-repeat',
              WebkitMaskRepeat: 'no-repeat',
              maskSize: 'contain',
              WebkitMaskSize: 'contain',
            }}
          />
        </div>
        <div className="flex items-center px-4 gap-2 pointer-events-none">
          <span className="text-xs font-mono uppercase tracking-widest text-muted-foreground">
            Longbow - Arma Dedicated Server Tool
          </span>
        </div>
      </div>
      <div className="flex">
        <Button
          variant="ghost"
          className="h-10 w-12 rounded-none hover:bg-white/5"
          onClick={() => appWindow.minimize()}
        >
          <Minus className="h-4 w-4" />
        </Button>
        <Button
          variant="ghost"
          className="h-10 w-12 rounded-none hover:bg-white/5"
          onClick={() => appWindow.toggleMaximize()}
        >
          <Square className="h-3 w-3" />
        </Button>
        <Button
          variant="ghost"
          className="h-10 w-12 rounded-none hover:bg-destructive/80 hover:text-white"
          onClick={() => appWindow.close()}
        >
          <X className="h-4 w-4" />
        </Button>

      </div>
    </div>
  );
}