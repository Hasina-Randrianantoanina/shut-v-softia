export const formatDateForInput = (dateString) => {
  if (!dateString) return "";

  let date;
  if (typeof dateString === "string") {
    date = new Date(dateString);
  } else if (dateString instanceof Date) {
    date = dateString;
  } else {
    console.error(`Invalid date format: ${dateString}`);
    return "";
  }
  if (isNaN(date.getTime())) {
    console.error(`Invalid date: ${dateString}`);
    return "";
  }
  const year = date.getFullYear();
  const month = (date.getMonth() + 1).toString().padStart(2, "0");
  const day = date.getDate().toString().padStart(2, "0");
  const hours = date.getHours().toString().padStart(2, "0");
  const minutes = date.getMinutes().toString().padStart(2, "0");

  return `${year}-${month}-${day}T${hours}:${minutes}`;
};

export const calculateDuration = (start, end) => {
  if (!start || !end) return 0;

  const startDate = new Date(start);
  const endDate = new Date(end);

  const diff = endDate - startDate;
  return Math.floor(diff / 1000);
};

export const formatDurationForDisplay = (durationInSeconds) => {
  const days = Math.floor(durationInSeconds / (24 * 60 * 60));
  const hours = Math.floor((durationInSeconds % (24 * 60 * 60)) / (60 * 60));
  const minutes = Math.floor((durationInSeconds % (60 * 60)) / 60);
  const seconds = durationInSeconds % 60;

  return `${days}j ${hours}h ${minutes}m ${seconds}s`;
};

export function formatDateForDisplay(dateString) {
  if (!dateString) return "";
  const date = new Date(dateString);
  return date.toLocaleDateString("fr-FR", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit",
  });
}

export function formatDateForDateTimeLocal(dateString) {
  if (!dateString) return "";
  const date = new Date(dateString);
  return date
    .toLocaleString("sv-SE", {
      year: "numeric",
      month: "2-digit",
      day: "2-digit",
      hour: "2-digit",
      minute: "2-digit",
      second: "2-digit",
    })
    .replace(" ", "T")
    .slice(0, 16);
}

export const compareDates = (valueA, valueB) => {
  const dateA = new Date(valueA + "Z").getTime();
  const dateB = new Date(valueB + "Z").getTime();
  return dateB - dateA;
};

export function formatDateWithTimezone(date) {
  const offset = date.getTimezoneOffset() * -1;
  const offsetHours = String(Math.floor(Math.abs(offset) / 60)).padStart(
    2,
    "0"
  );
  const offsetMinutes = String(Math.abs(offset) % 60).padStart(2, "0");
  const timezoneOffset =
    offset >= 0
      ? `+${offsetHours}:${offsetMinutes}`
      : `-${offsetHours}:${offsetMinutes}`;
  const localISOString = date.toISOString().slice(0, 19) + timezoneOffset;
  return localISOString;
}
