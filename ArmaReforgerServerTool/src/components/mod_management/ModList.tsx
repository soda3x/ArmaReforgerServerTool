import { Mod } from "@/types";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Draggable, Droppable } from "@hello-pangea/dnd";
import { GripVertical, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";

type ModListProps = {
  id: string;
  title: string;
  description: string;
  mods: Mod[];
  isActiveList?: boolean;
  onDelete: (id: string) => void;
};

export default function ModList({ id, title, description, mods, isActiveList = false, onDelete }: ModListProps) {
  return (
    <Card className={`h-full flex flex-col ${isActiveList ? "border-primary/50" : ""}`}>
      <CardHeader className="pb-3 border-b">
        <div className="flex items-center justify-between">
          <CardTitle>{title}</CardTitle>
        </div>
        <CardDescription>{description}</CardDescription>
      </CardHeader>

      <CardContent className="flex-1 p-2 min-h-0">
        <Droppable droppableId={id}>
          {(provided, snapshot) => (
            <ScrollArea
              className="h-full w-full rounded-md"
              {...provided.droppableProps}
              ref={provided.innerRef}
            >
              <div
                className={`min-h-[400px] p-2 rounded-md transition-colors ${snapshot.isDraggingOver ? "bg-muted/50" : ""
                  }`}
              >
                {mods.length === 0 && (
                  <div className="h-full flex items-center justify-center text-sm text-muted-foreground italic mt-8">
                    Empty
                  </div>
                )}

                {mods.map((mod, index) => (
                  <Draggable key={mod.id} draggableId={mod.id} index={index}>
                    {(provided, snapshot) => (
                      <div
                        ref={provided.innerRef}
                        {...provided.draggableProps}
                        className={`flex items-center gap-2 p-3 mb-2 bg-card border rounded-md shadow-sm transition-shadow ${snapshot.isDragging ? "shadow-md ring-1 ring-primary border-primary" : ""
                          }`}
                      >
                        <div
                          {...provided.dragHandleProps}
                          className="p-1 cursor-grab active:cursor-grabbing hover:bg-muted rounded text-muted-foreground shrink-0"
                        >
                          <GripVertical className="h-4 w-4" />
                        </div>

                        <div className="flex items-center gap-2 flex-1 min-w-0 overflow-hidden">
                          <span className="font-medium text-sm truncate">{mod.name}</span>
                          <Badge variant="outline" className="text-[10px] h-5 px-1.5 font-mono text-muted-foreground">
                            {mod.version}
                          </Badge>
                        </div>

                        {isActiveList && (
                          <span className="ml-auto text-xs text-muted-foreground font-mono shrink-0">
                            {index + 1}
                          </span>
                        )}
                        <Button
                          variant="ghost"
                          size="icon"
                          // Styling makes it subtle until hovered, then it turns red
                          className="h-7 w-7 text-muted-foreground hover:text-destructive hover:bg-destructive/10 shrink-0"
                          onClick={() => onDelete(mod.id)}
                        >
                          <Trash2 className="h-4 w-4" />
                          <span className="sr-only">Delete {mod.name}</span>
                        </Button>
                      </div>
                    )}
                  </Draggable>
                ))}
                {provided.placeholder}
              </div>
            </ScrollArea>
          )}
        </Droppable>
      </CardContent>
    </Card>
  );
}