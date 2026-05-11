import { Cog } from "lucide-react";
import { Card, CardContent } from "@/components/ui/card";
import { Accordion, AccordionContent, AccordionItem, AccordionTrigger } from "../ui/accordion";
import { Field, FieldLabel, FieldGroup, FieldLegend, FieldSet, FieldDescription } from "@/components/ui/field";
import { Input } from "@/components/ui/input";

const serverParamGroups = [
  {
    value: 'general',
    trigger: 'General',
    content:
      <>
        <FieldGroup>
          <FieldSet>
            <FieldLegend>General</FieldLegend>
            <FieldDescription>General Server Parameters</FieldDescription>
          </FieldSet>
        </FieldGroup>
        <FieldGroup>
          <Field>
            <FieldLabel htmlFor="server-name">Server Name</FieldLabel>
            <Input
                  id="server-name"
                  placeholder="My Longbow Arma Server"
                  required
                />
          </Field>
        </FieldGroup>
      </>
  }
]

export default function ServerInfo() {
  return (
    <div className="col-span-3 space-y-4">
      <div className="flex items-center gap-2 text-xs font-mono text-muted-foreground">
        <Cog className="h-3 w-3 text-brand-orange" /> SERVER_CONFIGURATION
      </div>
      <Card className="bg-card border-white/5">
        <CardContent className="p-4">
          <Accordion multiple >
            {serverParamGroups.map((item) => (
              <AccordionItem key={item.value} value={item.value}>
                <AccordionTrigger>{item.trigger}</AccordionTrigger>
                <AccordionContent>{item.content}</AccordionContent>
              </AccordionItem>
            ))}
          </Accordion>
        </CardContent>
      </Card>
    </div>
  );
}