using AutoMapper;
using Google.Apis.Drive.v3;
using Google.Apis.Upload;
using System.Net;
using ZonefyDotnet.Common;
using ZonefyDotnet.DTOs;
//using ZonefyUser = ZonefyDotnet.Entities.User;
//using ZonefyProp = ZonefyDotnet.Entities.HouseProperty;
//using ZonefyPropStat = ZonefyDotnet.Entities.PropertyStatistics;
using ZonefyDotnet.Entities;
using ZonefyDotnet.Helpers;
using ZonefyDotnet.Repositories.Interfaces;
using ZonefyDotnet.Services.Interfaces;

namespace ZonefyDotnet.Services.Implementations
{
    public class HousePropertyService : IHousePropertyService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<HouseProperty> _propertyRepository;
        private readonly IRepository<PropertyStatistics> _propertyStatisticsRepository;
        private readonly IMapper _mapper;
        private readonly IS3Service _s3Service;

        public HousePropertyService(
            IS3Service s3Service,
            IRepository<User> userRepository,
            IRepository<HouseProperty> propertyRepository,
            IRepository<PropertyStatistics> propertyStatisticsRepository,
            IMapper mapper)
        {
            _s3Service = s3Service;
            _userRepository = userRepository;
            _propertyRepository = propertyRepository;
            _propertyStatisticsRepository = propertyStatisticsRepository;
            _mapper = mapper;
        }

        public async Task<SuccessResponse<GetHousePropertyDTO>> CreateHouseProperty(CreateHousePropertyDTO model)
        {
            var findUser = await _userRepository.FirstOrDefault(x => x.Email == model.CreatorEmail);

            if (findUser == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            model.CheckInTime = DateTime.SpecifyKind(model.CheckInTime, DateTimeKind.Utc);
            model.CheckOutTime = DateTime.SpecifyKind(model.CheckOutTime, DateTimeKind.Utc);


            var newProperty = _mapper.Map<HouseProperty>(model);

            await _propertyRepository.AddAsync(newProperty);
            await _propertyRepository.SaveChangesAsync();

            var newPropertyResponse = _mapper.Map<GetHousePropertyDTO>(newProperty);

            return new SuccessResponse<GetHousePropertyDTO>
            {
                Data = newPropertyResponse,
                Code = 201,
                Message = ResponseMessages.NewPropertyCreated,
                ExtraInfo = "",
            };
        }

        public async Task<SuccessResponse<string>> DeleteHouseProperty(Guid id)
        {
            var findProperty = await _propertyRepository.FirstOrDefault(x=> x.Id == id);

            if (findProperty == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.PropertyNotFound);

            if(findProperty.PropertyImageUrl?.Count() > 0){
                foreach (var fileId in findProperty.PropertyImageUrl)
                {
                    try
                    {
                        await _s3Service.DeleteFileAsync(fileId);
                    }
                    catch (Exception ex)
                    {
                        // Log the error for debugging
                        Console.WriteLine(ex+ $"Failed to delete file with ID: {fileId}");
                        // You can choose to continue or rethrow depending on your requirements
                    }
                }
            }

            var name = findProperty.PropertyName;
            _propertyRepository.Remove(findProperty);
            await _propertyRepository.SaveChangesAsync();

            return new SuccessResponse<string>
            {
                Data = $"{name} House property has been deleted",
                Code = 200,
                Message = ResponseMessages.PropertyDeleted,
                ExtraInfo = "",
            };
            
        }

        public async Task<SuccessResponse<string>> DeletePropertyImageAsync(string fileId, string userEmail, Guid propertyId)
        {
            var findUser = await _userRepository.FirstOrDefault(x => x.Email == userEmail);

            if (findUser == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            var findProperty = await _propertyRepository.FirstOrDefault(x => x.Id == propertyId);

            if (findProperty == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.PropertyNotFound);


                await _s3Service.DeleteFileAsync(fileId);

                findProperty.PropertyImageUrl.Remove(fileId);
                await _propertyRepository.SaveChangesAsync();


                return new SuccessResponse<string>
                {
                    Data = $"File with ID {fileId} has been deleted.",
                    Code = 200,
                    Message = ResponseMessages.ImageDeleted,
                    ExtraInfo = "File deleted successfully."
                };
            
        }

        public async Task<Stream> DownloadPropertyImageAsync(string fileId, string userEmail, Guid propertyId)
        {
            var findUser = await _userRepository.FirstOrDefault(x => x.Email == userEmail);
            if (findUser == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            var findProperty = await _propertyRepository.FirstOrDefault(x => x.Id == propertyId);
            if (findProperty == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.PropertyNotFound);

            return await _s3Service.DownloadFileAsync(fileId);
        }

        public async Task<SuccessResponse<PaginatedResponse<GetHousePropertyDTO>>> GetAllHouseProperties(int pageNumber = 1)
        {
            int pageSize = 30;
            // Ensure pageNumber is at least 1
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            int skip = (pageNumber - 1) * pageSize;
            var allProperties = await _propertyRepository.GetAllPaginatedAsync(skip, pageSize, p => p.CreatedAt);
            var totalCount = await _propertyRepository.CountAsync(_ => true);
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);


            var propertiesResponse = _mapper.Map<IEnumerable<GetHousePropertyDTO>>(allProperties);

            var paginatedResponse = new PaginatedResponse<GetHousePropertyDTO>
            {
                Data = propertiesResponse,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalRecords = totalCount
            };

            return new SuccessResponse<PaginatedResponse<GetHousePropertyDTO>>
            {
                Data = paginatedResponse,
                Code = 200,
                Message = ResponseMessages.FetchedSuccesss,
                ExtraInfo = "",
            };
        }

        public async Task<SuccessResponse<PaginatedResponse<GetHousePropertyDTO>>> GetAllHousePropertiesByEmail(string email, int pageNumber = 1)
        {
            int pageSize = 30;
            // Ensure pageNumber is at least 1
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            int skip = (pageNumber - 1) * pageSize;

            // Retrieve the total count of properties for the given email for pagination metadata
            int totalCount = await _propertyRepository.CountAsync(x => x.CreatorEmail == email);

            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);


            // Fetch the paginated data
            var allProperties = await _propertyRepository.FindPaginatedAsync(x => x.CreatorEmail == email, skip, pageSize, p => p.CreatedAt);

            // Map the properties to DTOs
            var propertiesResponse = _mapper.Map<IEnumerable<GetHousePropertyDTO>>(allProperties);

            // Wrap in paginated response
            var paginatedResponse = new PaginatedResponse<GetHousePropertyDTO>
            {
                Data = propertiesResponse,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalRecords = totalCount
            };

            return new SuccessResponse<PaginatedResponse<GetHousePropertyDTO>>
            {
                Data = paginatedResponse,
                Code = 200,
                Message = ResponseMessages.FetchedSuccesss,
                ExtraInfo = ""
            };
        }

        public async Task<SuccessResponse<PaginatedResponse<GetPropertyStatisticDTO>>> GetAllUserPropertyStatisticsByEmail(string email, int pageNumber)
        {
            int pageSize = 30;
            // Ensure pageNumber is at least 1
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            int skip = (pageNumber - 1) * pageSize;

            // Retrieve the total count of properties for the given email for pagination metadata
            int totalCount = await _propertyStatisticsRepository.CountAsync(x => x.CreatorEmail == email || x.UserEmail == email);

            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);


            // Fetch the paginated data
            var allProperties = await _propertyStatisticsRepository.FindPaginatedAsync(x => x.CreatorEmail == email || x.UserEmail == email, skip, pageSize, p => p.CreatedAt);


            var propertiesResponse = _mapper.Map<IEnumerable<GetPropertyStatisticDTO>>(allProperties);

            var paginatedResponse = new PaginatedResponse<GetPropertyStatisticDTO>
            {
                Data = propertiesResponse,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalRecords = totalCount
            };

            return new SuccessResponse<PaginatedResponse<GetPropertyStatisticDTO>>
            {
                Data = paginatedResponse,
                Code = 200,
                Message = ResponseMessages.FetchedSuccesss,
                ExtraInfo = "",
            };
        }

        public async Task<SuccessResponse<PaginatedResponse<GetPropertyStatisticDTO>>> GetPropertyStatisticsById(Guid id, int pageNumber = 1)
        {
            int pageSize = 30;
            // Ensure pageNumber is at least 1
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            int skip = (pageNumber - 1) * pageSize;

            // Retrieve the total count of properties for the given email for pagination metadata
            int totalCount = await _propertyStatisticsRepository.CountAsync(x => x.PropertyId == id);

            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);


            // Fetch the paginated data
            var allProperties = await _propertyStatisticsRepository.FindPaginatedAsync(x => x.PropertyId == id, skip, pageSize, p => p.CreatedAt);


            var propertiesResponse = _mapper.Map<IEnumerable<GetPropertyStatisticDTO>>(allProperties);

            var paginatedResponse = new PaginatedResponse<GetPropertyStatisticDTO>
            {
                Data = propertiesResponse,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalRecords = totalCount
            };

            return new SuccessResponse<PaginatedResponse<GetPropertyStatisticDTO>>
            {
                Data = paginatedResponse,
                Code = 200,
                Message = ResponseMessages.FetchedSuccesss,
                ExtraInfo = "",
            };
        }

        public async Task<SuccessResponse<GetHousePropertyDTO>> UpdateHouseProperty(UpdateHousePropertyDTO model)
        {
            var findProperty = await _propertyRepository.FirstOrDefault(x => x.Id == model.Id);

            if (findProperty == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.PropertyNotFound);

            await Functions.UpdateProperties(model, findProperty);
            findProperty.UpdatedAt = DateTime.UtcNow;

            var response = _mapper.Map<GetHousePropertyDTO>(findProperty);

            await _propertyRepository.SaveChangesAsync();

            return new SuccessResponse<GetHousePropertyDTO>
            {
                Data = response,
                Code = 200,
                Message = ResponseMessages.HouseUpdated,
                ExtraInfo = "",

            };
        }

        public async Task<SuccessResponse<List<string>>> UploadPropertyImages(List<IFormFile> files, Guid propertyId)
        {
            if (files == null || files.Count == 0)
            {
                throw new RestException(HttpStatusCode.BadRequest, "No files uploaded.");
            }

            
            var findProperty = await _propertyRepository.FirstOrDefault(x => x.Id == propertyId);

            if (findProperty == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.PropertyNotFound);

            var uploadedImgs = await _s3Service.UploadFileAsync(files);

            if (uploadedImgs != null && uploadedImgs.Count > 0)
            {
                findProperty.PropertyImageUrl ??= new List<string>();
                findProperty.PropertyImageUrl.AddRange(uploadedImgs);
                await _propertyRepository.SaveChangesAsync();
            }

            return new SuccessResponse<List<string>>
            {
                Data = uploadedImgs,
                Code = 200,
                Message = ResponseMessages.ImageUploaded,
                ExtraInfo = "All files uploaded successfully."
            };            
        }
    }
}
