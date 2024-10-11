import React, { useState, useEffect } from "react";
import Calendar from "react-calendar";
import "@/styles/custom-calendar.css";
import "react-calendar/dist/Calendar.css";

const DateRangeSelector = ({
  dateRange,
  onDateRangeChange,
  selectedStation,
}) => {
  const [selectedYear, setSelectedYear] = useState(new Date().getFullYear());
  const [selectedWeek, setSelectedWeek] = useState("all");
  const [selectedMonth, setSelectedMonth] = useState("all");
  const [calendarDate, setCalendarDate] = useState(new Date());

  const years = Array.from(
    { length: new Date().getFullYear() - 1999 },
    (_, i) => 2000 + i
  );

  const months = [
    "all",
    "Janvier",
    "Février",
    "Mars",
    "Avril",
    "Mai",
    "Juin",
    "Juillet",
    "Août",
    "Septembre",
    "Octobre",
    "Novembre",
    "Décembre",
  ];

  const getWeeksInYear = (year) => {
    const weeks = [];
    const firstDayOfYear = new Date(year, 0, 1);
    const lastDayOfYear = new Date(year, 11, 31);

    let currentDate = new Date(firstDayOfYear);
    let weekNumber = 1;

    while (currentDate <= lastDayOfYear) {
      weeks.push(weekNumber);
      currentDate.setDate(currentDate.getDate() + 7);
      weekNumber++;
    }
    return weeks;
  };

  const weeks = ["all", ...getWeeksInYear(selectedYear)];

  const updateDateRange = (startDate, endDate) => {
    onDateRangeChange([startDate, endDate]);
    setCalendarDate(startDate);
  };

  useEffect(() => {
    let startDate, endDate;

    if (selectedMonth !== "all") {
      const monthIndex = months.indexOf(selectedMonth) - 1;
      startDate = new Date(selectedYear, monthIndex, 1);
      endDate = new Date(selectedYear, monthIndex + 1, 0);
    } else if (selectedWeek !== "all") {
      startDate = new Date(selectedYear, 0, 1);
      startDate.setDate(startDate.getDate() + (selectedWeek - 1) * 7);
      endDate = new Date(startDate);
      endDate.setDate(endDate.getDate() + 6);
    } else {
      startDate = new Date(selectedYear, 0, 1);
      endDate = new Date(selectedYear, 11, 31);
      if (selectedYear === new Date().getFullYear()) {
        endDate = new Date();
      }
    }

    updateDateRange(startDate, endDate);
  }, [selectedYear, selectedWeek, selectedMonth]);

  const handleCalendarChange = (value) => {
    if (Array.isArray(value)) {
      onDateRangeChange(value);
      setSelectedYear(value[0].getFullYear());
      setSelectedMonth("all");
      setSelectedWeek("all");
    }
  };

  const handleCalendarActiveStartDateChange = ({ activeStartDate }) => {
    setCalendarDate(activeStartDate);
    setSelectedYear(activeStartDate.getFullYear());
  };

  const capitalizeFirstTwo = (str) => {
    if (!str) return "";
    if (str.length <= 2) {
      return str.toUpperCase();
    }
    return str.slice(0, 2).toUpperCase() + str.slice(2).toLowerCase();
  };

  return (
    <div className="w-full lg:w-2/5">
      {selectedStation && (
        <div className="flex justify-start mb-4 text-2xl font-bold item-center text-atoli_blue">
          Station {capitalizeFirstTwo(selectedStation)}
        </div>
      )}
      <h2 className="mb-4 text-xl font-bold">Choisir une période</h2>
      <div className="flex flex-wrap justify-start gap-4 mb-4 font-bold">
        <select
          value={selectedYear}
          onChange={(e) => {
            const newYear = Number(e.target.value);
            setSelectedYear(newYear);
            setSelectedWeek("all");
            setSelectedMonth("all");
            updateDateRange(new Date(newYear, 0, 1), new Date(newYear, 11, 31));
          }}
          className="p-2 border rounded-xl border-atoli_blue"
        >
          {years.map((year) => (
            <option key={year} value={year}>
              {year}
            </option>
          ))}
        </select>
        <select
          value={selectedMonth}
          onChange={(e) => {
            const newMonth = e.target.value;
            setSelectedMonth(newMonth);
            setSelectedWeek("all");
            if (newMonth !== "all") {
              const monthIndex = months.indexOf(newMonth) - 1;
              const startDate = new Date(selectedYear, monthIndex, 1);
              const endDate = new Date(selectedYear, monthIndex + 1, 0);
              updateDateRange(startDate, endDate);
            } else {
              updateDateRange(new Date(selectedYear, 0, 1), new Date(selectedYear, 11, 31));
            }
          }}
          className="p-2 border rounded-xl border-atoli_blue"
        >
          <option value="all">Tous les mois</option>
          {months.slice(1).map((month) => (
            <option key={month} value={month}>
              {month}
            </option>
          ))}
        </select>
        <select
          value={selectedWeek}
          onChange={(e) => {
            const newWeek = e.target.value;
            setSelectedWeek(newWeek);
            setSelectedMonth("all");
            if (newWeek !== "all") {
              const weekStart = new Date(selectedYear, 0, 1);
              weekStart.setDate(weekStart.getDate() + (Number(newWeek) - 1) * 7);
              const weekEnd = new Date(weekStart);
              weekEnd.setDate(weekEnd.getDate() + 6);
              updateDateRange(weekStart, weekEnd);
            } else {
              updateDateRange(new Date(selectedYear, 0, 1), new Date(selectedYear, 11, 31));
            }
          }}
          className="p-2 border rounded-xl border-atoli_blue"
        >
          <option value="all">Toutes les semaines</option>
          {weeks.slice(1).map((week) => (
            <option key={week} value={week}>
              Semaine {week}
            </option>
          ))}
        </select>
      </div>
      <div className="flex justify-start mb-4">
        <Calendar
          onChange={handleCalendarChange}
          onActiveStartDateChange={handleCalendarActiveStartDateChange}
          value={dateRange}
          selectRange={true}
          maxDate={new Date()}
          minDate={new Date(2000, 0, 1)}
          locale="fr-FR"
          className="w-full p-2 mr-4 border custom-calendar border-atoli_blue rounded-xl"
          navigationLabel={({ date }) =>
            date.toLocaleString("fr-FR", {
              month: "long",
              year: "numeric",
            })
          }
          activeStartDate={calendarDate}
        />
      </div>
      <div className="flex justify-center">
        <span className="text-lg font-bold text-atoli_blue">
          Début : {dateRange[0].toLocaleDateString()}
        </span>
        <span className="ml-8 text-lg font-bold text-atoli_blue">
          Fin : {dateRange[1].toLocaleDateString()}
        </span>
      </div>
    </div>
  );
};

export default DateRangeSelector;