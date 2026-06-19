using Dsw2026Ej15.Data.Dtos;
using Dsw2026Ej15.Domain.Entities;
using Dsw2026Ej15.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Dsw2026Ej15.Data
{
    public class PersistanceInMemory : IPersistance
    {
        private List<Speciality> _specialities = [];
        private List<Doctor> _doctors = [];

        public PersistanceInMemory()
        {
            LoadSpecialities();
        }

        public IEnumerable<Speciality> GetSpecialities()
        {
            return _specialities;
        }

        public Speciality? GetSpecialityById(Guid id)
        {
            foreach (Speciality speciality in _specialities)
            {
                if (speciality.Id == id)
                {
                    return speciality;
                }
            }

            return null;
        }

        public IEnumerable<Doctor> GetActiveDoctors()
        {
            List<Doctor> activeDoctors = new List<Doctor>();

            foreach (Doctor doctor in _doctors)
            {
                if (doctor.IsActive)
                {
                    activeDoctors.Add(doctor);
                }
            }

            return activeDoctors;
        }

        public Doctor? GetActiveDoctorById(Guid id)
        {
            foreach (Doctor doctor in _doctors)
            {
                if (doctor.Id == id && doctor.IsActive)
                {
                    return doctor;
                }
            }

            return null;
        }

        public void AddDoctor(Doctor doctor)
        {
            _doctors.Add(doctor);
        }

        public void UpdateDoctor(Doctor doctor)
        {
            for (int i = 0; i < _doctors.Count; i++)
            {
                if (_doctors[i].Id == doctor.Id)
                {
                    _doctors[i] = doctor;
                    break;
                }
            }
        }

        private void LoadSpecialities()
        {
            try
            {
                string jsonPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "specialities.json");

                string json = File.ReadAllText(jsonPath);

                List<SpecialityDto> specialities =
                    JsonSerializer.Deserialize<List<SpecialityDto>>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        }) ?? [];

                _specialities = specialities
                    .Select(s => new Speciality(s.Name, s.Description, s.Id))
                    .ToList();
            }
            catch (Exception)
            {
            }
        }
    }
}

