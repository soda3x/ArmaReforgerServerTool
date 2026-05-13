import { FileDown, FileUp, Plus, Puzzle } from "lucide-react";
import React, { useState, useEffect } from "react";
import { DragDropContext, DropResult } from "@hello-pangea/dnd";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { ArrowRight, ArrowLeft } from "lucide-react";
import { Mod } from "@/types";
import ModList from "@/components/mod_management/ModList";
import { Dialog, DialogClose, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle, DialogTrigger } from "../ui/dialog";
import { Field, FieldDescription, FieldGroup } from "../ui/field";
import { Label } from "../ui/label";


// Initial dummy data
const INITIAL_AVAILABLE: Mod[] = [
  { id: "mod-1", name: "Better Textures", version: "1.2.4" },
  { id: "mod-2", name: "Infinite Ammo", version: "latest" },
  { id: "mod-3", name: "Custom UI", version: "2.0.0-beta" },
];

export default function ModManager() {
  const [availableMods, setAvailableMods] = useState<Mod[]>(INITIAL_AVAILABLE);
  const [enabledMods, setEnabledMods] = useState<Mod[]>([]);
  const [newModIdInput, setNewModIdInput] = useState("");
  const [newModNameInput, setNewModNameInput] = useState("");
  const [newModVersionInput, setNewModVersionInput] = useState("");

  const [isBrowser, setIsBrowser] = useState(false);
  useEffect(() => setIsBrowser(true), []);

  const handleAddMod = (e: React.FormEvent) => {
    e.preventDefault();
    if (!newModIdInput.trim()) return;
    if (!newModNameInput.trim()) return;

    const newMod: Mod = {
      id: newModIdInput.trim(),
      name: newModNameInput.trim(),
      // Fallback to "latest" if the version input is empty
      version: newModVersionInput.trim() || "latest",
    };

    setAvailableMods((prev) => [...prev, newMod]);
    setNewModIdInput("");
    setNewModNameInput("");
    setNewModVersionInput("");
  };

  const handleEnableAll = () => {
    setEnabledMods((prev) => [...prev, ...availableMods]);
    setAvailableMods([]);
  };

  const handleDisableAll = () => {
    setAvailableMods((prev) => [...prev, ...enabledMods]);
    setEnabledMods([]);
  };

  const handleDeleteMod = (modId: string) => {
    // Filter it out of whichever list currently holds it
    setAvailableMods((prev) => prev.filter((mod) => mod.id !== modId));
    setEnabledMods((prev) => prev.filter((mod) => mod.id !== modId));
  };

  const onDragEnd = (result: DropResult) => {
    const { source, destination } = result;
    if (!destination) return;
    if (source.droppableId === destination.droppableId && source.index === destination.index) return;

    const getList = (id: string) => (id === "available" ? availableMods : enabledMods);
    const setList = (id: string, newList: Mod[]) => {
      if (id === "available") setAvailableMods(newList);
      else setEnabledMods(newList);
    };

    const sourceList = getList(source.droppableId);
    const destList = getList(destination.droppableId);

    if (source.droppableId === destination.droppableId) {
      const reorderedList = Array.from(sourceList);
      const [movedItem] = reorderedList.splice(source.index, 1);
      reorderedList.splice(destination.index, 0, movedItem);
      setList(source.droppableId, reorderedList);
    } else {
      const newSourceList = Array.from(sourceList);
      const newDestList = Array.from(destList);
      const [movedItem] = newSourceList.splice(source.index, 1);
      newDestList.splice(destination.index, 0, movedItem);

      setList(source.droppableId, newSourceList);
      setList(destination.droppableId, newDestList);
    }
  };

  if (!isBrowser) return null;

  return (
    <>
      <div className="flex items-center gap-2 text-xs font-mono text-muted-foreground">
        <Puzzle className="h-3 w-3 text-brand-orange" /> MOD_MANAGER
      </div>

      <div className="flex flex-col space-y-4 pt-2 h-full flex-1">
        <div className="flex justify-between">
          <div className="flex gap-4">
            <Dialog>
              <form>
                <DialogTrigger>
                  <Button variant="outline"><Plus /> Add Mod</Button>
                </DialogTrigger>
                <DialogContent>
                  <DialogHeader>
                    <DialogTitle>Add a Mod</DialogTitle>
                    <DialogDescription>
                      Add a mod from the Arma Workshop to your server
                    </DialogDescription>
                  </DialogHeader>
                  <FieldGroup>
                    <Field>
                      <Label htmlFor="mod-id">ID</Label>
                      <FieldDescription>This can be found on the Arma Workshop</FieldDescription>
                      <Input id="mod-id" name="mod-id" onChange={(e) => setNewModIdInput(e.target.value)} />
                    </Field>
                    <Field>
                      <Label htmlFor="mod-name">Name</Label>
                      {/* <FieldDescription>This can be whatever you want to call it, it does not need to match the Workshop</FieldDescription> */}
                      <Input id="mod-name" name="mod-name" onChange={(e) => setNewModNameInput(e.target.value)} />
                    </Field>
                    <Field>
                      <Label htmlFor="mod-version">Version</Label>
                      <Input id="mod-version" name="mod-version" placeholder="Leave to use latest version" onChange={(e) => setNewModVersionInput(e.target.value)} />
                    </Field>
                  </FieldGroup>
                  <DialogFooter>
                    <DialogClose>
                      <Button variant="outline">Cancel</Button>
                    </DialogClose>
                    <Button type="submit" onClick={() => handleAddMod}>Add</Button>
                  </DialogFooter>
                </DialogContent>
              </form>
            </Dialog>
            <Dialog>
              <form>
                <DialogTrigger>
                  <Button variant="outline"><FileDown /> Import Mod List</Button>
                </DialogTrigger>
                <DialogContent>
                  <DialogHeader>
                    <DialogTitle>Add a Mod</DialogTitle>
                    <DialogDescription>
                      Add a mod from the Arma Workshop to your server
                    </DialogDescription>
                  </DialogHeader>
                  <FieldGroup>
                    <Field>
                      <Label htmlFor="mod-id">ID</Label>
                      <FieldDescription>This can be found on the Arma Workshop</FieldDescription>
                      <Input id="mod-id" name="mod-id" onChange={(e) => setNewModIdInput(e.target.value)} />
                    </Field>
                    <Field>
                      <Label htmlFor="mod-name">Name</Label>
                      {/* <FieldDescription>This can be whatever you want to call it, it does not need to match the Workshop</FieldDescription> */}
                      <Input id="mod-name" name="mod-name" onChange={(e) => setNewModNameInput(e.target.value)} />
                    </Field>
                    <Field>
                      <Label htmlFor="mod-version">Version</Label>
                      <Input id="mod-version" name="mod-version" placeholder="Leave to use latest version" onChange={(e) => setNewModVersionInput(e.target.value)} />
                    </Field>
                  </FieldGroup>
                  <DialogFooter>
                    <DialogClose>
                      <Button variant="outline">Cancel</Button>
                    </DialogClose>
                    <Button type="submit" onClick={() => handleAddMod}>Add</Button>
                  </DialogFooter>
                </DialogContent>
              </form>
            </Dialog>
          </div>
          <Dialog>
            <form>
              <DialogTrigger>
                <Button variant="outline"><FileUp /> Export Mod List</Button>
              </DialogTrigger>
              <DialogContent>
                <DialogHeader>
                  <DialogTitle>Add a Mod</DialogTitle>
                  <DialogDescription>
                    Add a mod from the Arma Workshop to your server
                  </DialogDescription>
                </DialogHeader>
                <FieldGroup>
                  <Field>
                    <Label htmlFor="mod-id">ID</Label>
                    <FieldDescription>This can be found on the Arma Workshop</FieldDescription>
                    <Input id="mod-id" name="mod-id" onChange={(e) => setNewModIdInput(e.target.value)} />
                  </Field>
                  <Field>
                    <Label htmlFor="mod-name">Name</Label>
                    {/* <FieldDescription>This can be whatever you want to call it, it does not need to match the Workshop</FieldDescription> */}
                    <Input id="mod-name" name="mod-name" onChange={(e) => setNewModNameInput(e.target.value)} />
                  </Field>
                  <Field>
                    <Label htmlFor="mod-version">Version</Label>
                    <Input id="mod-version" name="mod-version" placeholder="Leave to use latest version" onChange={(e) => setNewModVersionInput(e.target.value)} />
                  </Field>
                </FieldGroup>
                <DialogFooter>
                  <DialogClose>
                    <Button variant="outline">Cancel</Button>
                  </DialogClose>
                  <Button type="submit" onClick={() => handleAddMod}>Add</Button>
                </DialogFooter>
              </DialogContent>
            </form>
          </Dialog>
        </div>

        <DragDropContext onDragEnd={onDragEnd}>
          <div className="grid md:grid-cols-[1fr_auto_1fr] gap-4 items-center flex flex-col flex-1 h-full">

            <ModList
              id="available"
              title="Available Mods"
              description="Drag to enable"
              mods={availableMods}
              onDelete={handleDeleteMod}
            />

            <div className="flex md:flex-col justify-center gap-2">
              <Button variant="outline" size="icon-lg" onClick={handleEnableAll} disabled={availableMods.length === 0}>
                <div className="flex flex-col">
                  <ArrowRight className="h-4 w-4 hidden md:block" />
                  <ArrowRight className="h-4 w-4 hidden md:block" />
                  <ArrowLeft className="h-4 w-4 md:hidden" />
                  <ArrowLeft className="h-4 w-4 md:hidden" />
                </div>
                <span className="sr-only">Enable All</span>
              </Button>
              <Button variant="outline" size="icon-lg" onClick={handleDisableAll} disabled={enabledMods.length === 0}>
                <div className="flex flex-col">
                  <ArrowLeft className="h-4 w-4 hidden md:block" />
                  <ArrowLeft className="h-4 w-4 hidden md:block" />
                  <ArrowRight className="h-4 w-4 md:hidden" />
                  <ArrowRight className="h-4 w-4 md:hidden" />
                </div>
                <span className="sr-only">Disable All</span>
              </Button>
            </div>

            <ModList
              id="enabled"
              title="Enabled Mods"
              description="Drag to reorder load priority"
              mods={enabledMods}
              isActiveList
              onDelete={handleDeleteMod}
            />

          </div>
        </DragDropContext>
      </div>
    </>
  );
}

