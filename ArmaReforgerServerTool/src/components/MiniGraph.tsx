import { Area, AreaChart, ResponsiveContainer, YAxis } from "recharts";

// FIXME: Mock data: In the future, this will come from Rust via Tauri Events
export const DEBUG_MINIGRAPH_MOCK_DATA = [
  { val: 10 }, { val: 15 }, { val: 8 }, { val: 22 },
  { val: 18 }, { val: 25 }, { val: 30 }, { val: 28 }
];

const DEFAULT_COLOUR="#ff6000"

export default function MiniGraph({ data, color }: { data: any[], color?: string }) {
  return (
    <div className="h-[40px] w-full mt-2 opacity-50">
      <ResponsiveContainer width="100%" height="100%">
        <AreaChart data={data}>
          <defs>
            <linearGradient id={`gradient-${color ? color : DEFAULT_COLOUR}`} x1="0" y1="0" x2="0" y2="1">
              <stop offset="5%" stopColor={color ? color : DEFAULT_COLOUR} stopOpacity={0.3} />
              <stop offset="95%" stopColor={color ? color: DEFAULT_COLOUR} stopOpacity={0} />
            </linearGradient>
          </defs>
          <Area
            type="monotone"
            dataKey="val"
            stroke={color ? color : DEFAULT_COLOUR}
            strokeWidth={1.5}
            fillOpacity={1}
            fill={`url(#gradient-${color ? color : DEFAULT_COLOUR})`}
            isAnimationActive={false} // Faster performance for real-time
          />
        </AreaChart>
      </ResponsiveContainer>
    </div>
  );
}