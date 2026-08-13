<script lang="ts">
  // Thin Svelte wrapper around Chart.js. Chosen over a hand-rolled SVG sparkline because it
  // gives free responsive resizing, axis/tooltip handling, and smooth updates for the rolling
  // 60-point FPS/Memory history — worth the added dependency for two live charts.
  import { onDestroy, onMount } from "svelte";
  import { Chart, LineController, LineElement, PointElement, LinearScale, CategoryScale, Filler, Tooltip } from "chart.js";
  import type { HistoryPoint } from "../stores";

  Chart.register(LineController, LineElement, PointElement, LinearScale, CategoryScale, Filler, Tooltip);

  interface Props {
    points: HistoryPoint[];
    color?: string;
    unit?: string;
    decimals?: number;
  }

  let { points, color = "#4f8cff", unit = "", decimals = 1 }: Props = $props();

  let canvas: HTMLCanvasElement;
  let chart: Chart | null = null;

  function labelsAndData(pts: HistoryPoint[]) {
    return {
      labels: pts.map(() => ""),
      data: pts.map((p) => p.v),
    };
  }

  onMount(() => {
    const { labels, data } = labelsAndData(points);
    chart = new Chart(canvas, {
      type: "line",
      data: {
        labels,
        datasets: [
          {
            data,
            borderColor: color,
            backgroundColor: color + "33",
            fill: true,
            tension: 0.3,
            pointRadius: 0,
            borderWidth: 2,
          },
        ],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        animation: false,
        scales: {
          x: { display: false },
          y: {
            display: true,
            grid: { color: "#2e333d" },
            ticks: { color: "#9aa1ac", callback: (v) => `${Number(v).toFixed(decimals)}${unit}` },
          },
        },
        plugins: {
          tooltip: {
            callbacks: {
              label: (ctx) => `${Number(ctx.parsed.y).toFixed(decimals)}${unit}`,
            },
          },
        },
      },
    });
  });

  onDestroy(() => {
    chart?.destroy();
  });

  $effect(() => {
    if (!chart) return;
    const { labels, data } = labelsAndData(points);
    chart.data.labels = labels;
    chart.data.datasets[0].data = data;
    chart.update("none");
  });
</script>

<div style="position:relative; height:180px; width:100%;">
  <canvas bind:this={canvas}></canvas>
</div>
