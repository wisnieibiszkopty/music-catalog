<script setup lang="ts">
import type { Track } from '@/services/api/catalog-service.ts'
import type { TableColumn } from '@nuxt/ui/components/Table.vue'
import { h } from 'vue'

const props = defineProps<{
  tracks: Track[]
}>()

const columns: TableColumn<Track>[] = [
  {
    accessorKey: 'trackNumber',
    header: 'No.',
    size: 50,
  },
  {
    accessorKey: 'name',
    header: 'Title',
    cell: ({ row }) => {
      return h('span', { class: 'font-medium text-gray-900 dark:text-white' }, row.getValue('name'))
    },
  },
  {
    accessorKey: 'durationMs',
    header: 'Duration',
    cell: ({ row }) => formatDuration(row.getValue('durationMs')),
  },
]

function formatDuration(ms: number) {
  const minutes = Math.floor(ms / 60000)
  const seconds = Math.floor((ms % 60000) / 1000)
  return `${minutes}:${seconds.toString().padStart(2, '0')}`
}
</script>

<template>
  <UTable
    :data="props.tracks"
    :columns="columns"
    :ui="{
      thead: 'border-b border-gray-200 dark:border-gray-800',
      tr: { base: 'hover:bg-gray-50/50 dark:hover:bg-gray-800/50 transition-colors' },
    }"
  />
</template>

<style scoped></style>
