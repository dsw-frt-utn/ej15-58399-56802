using Dsw2026Ej15.Api.Models;
using Dsw2026Ej15.Domain.Entities;
using Dsw2026Ej15.Domain.Exceptions;
using Dsw2026Ej15.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace Dsw2026Ej15.Api.Controllers
{
    [ApiController]
    [Route("api/doctors")]
    public class DoctorsController : ControllerBase
    {
        private readonly IPersistance _persistence;

        public DoctorsController(IPersistance persistence)
        {
            _persistence = persistence;
        }

        [HttpPost]
        public IActionResult CreateDoctor([FromBody] DoctorModel.Request request)
        {
            if (request is null)
            {
                throw new ValidationException("El cuerpo de la solicitud es requerido");
            }

            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.LicenseNumber))
            {
                throw new ValidationException("Nombre y matricula son requeridos");
            }

            Speciality? speciality = _persistence.GetSpecialityById(request.SpecialityId);

            if (speciality is null)
            {
                throw new ValidationException("Especialidad no existe");
            }

            Doctor doctor = new Doctor(request.Name.Trim(), request.LicenseNumber.Trim(), speciality);

            _persistence.AddDoctor(doctor);

            DoctorModel.Response response = new DoctorModel.Response(
                doctor.Id,
                doctor.Name,
                doctor.LicenseNumber,
                doctor.Speciality.Name);

            return Created($"/api/doctors/{doctor.Id}", response);
        }

        [HttpGet]
        public IActionResult GetDoctors()
        {
            IEnumerable<Doctor> doctors = _persistence.GetActiveDoctors();

            List<DoctorModel.Response> response = new List<DoctorModel.Response>();

            foreach (Doctor doctor in doctors)
            {
                response.Add(new DoctorModel.Response(
                    doctor.Id,
                    doctor.Name,
                    doctor.LicenseNumber,
                    doctor.Speciality.Name));
            }

            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetDoctorById(Guid id)
        {
            Doctor? doctor = _persistence.GetActiveDoctorById(id);

            if (doctor is null)
            {
                return NotFound();
            }

            DoctorModel.Response response = new DoctorModel.Response(
                doctor.Id,
                doctor.Name,
                doctor.LicenseNumber,
                doctor.Speciality.Name);

            return Ok(response);
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteDoctor(Guid id)
        {
            Doctor? doctor = _persistence.GetActiveDoctorById(id);

            if (doctor is null)
            {
                return NotFound();
            }

            doctor.IsActive = false;
            _persistence.UpdateDoctor(doctor);

            return NoContent();
        }
    }
}
