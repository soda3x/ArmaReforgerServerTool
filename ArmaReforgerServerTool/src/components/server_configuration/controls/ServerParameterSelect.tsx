import { Select, SelectContent, SelectGroup, SelectItem, SelectLabel, SelectTrigger, SelectValue } from "@/components/ui/select";

type SelectProps = {
  placeholder?: string,
  label?: string,
  items: string[]
}
export default function ServerParameterSelect({ placeholder, label, items }: SelectProps) {
  return (
    <Select>
      <SelectTrigger className="w-full max-w-48">
        <SelectValue placeholder={placeholder} />
      </SelectTrigger>
      <SelectContent>
        <SelectGroup>
          <SelectLabel>{label}</SelectLabel>
          {items.map((item, _) => (
            <SelectItem value={item}>{item}</SelectItem>
          ))}
        </SelectGroup>
      </SelectContent>
    </Select>
  );
}