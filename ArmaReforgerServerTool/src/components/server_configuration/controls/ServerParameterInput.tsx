import { Input } from "@/components/ui/input";
import { Field, FieldGroup, FieldLabel } from "@/components/ui/field";
import { ReactNode } from "react";

type InputProps = {
  id: string,
  label: ReactNode,
  placeholder?: string,
  type?: "text" | "number",
  defaultValue?: string | number
  minValue?: number,
  maxValue?: number,
  required?: boolean,
  hint?: ReactNode
}
export default function ServerParameterInput({ id, label, placeholder, type = "text", defaultValue, minValue, maxValue, required = false, hint }: InputProps) {
  return (
      <FieldGroup>
        <Field>
          <FieldLabel htmlFor={id}>{label}</FieldLabel>
          <Input
            id={id}
            placeholder={placeholder}
            type={type}
            min={minValue}
            max={maxValue}
            defaultValue={defaultValue}
            required={required}
          />
        </Field>
      </FieldGroup>
  );
}