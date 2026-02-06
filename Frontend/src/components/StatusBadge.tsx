export function StatusBadge({ status }: { status: string }) {
  const classMap: Record<string, string> = {
    success: 'bg-green-500 text-white',
    completed: 'bg-green-500 text-white',
    pending: 'bg-yellow-400 text-gray-900',
    failed: 'bg-red-500 text-white',
  };
  const cls = classMap[status.toLowerCase()] || 'bg-gray-500 text-white';
  return (
    <span className={`px-2 py-1 rounded text-xs font-medium ${cls}`}>
      {status}
    </span>
  );
}
