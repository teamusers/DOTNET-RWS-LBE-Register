using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RWS_LBE_Register.Data;
using RWS_LBE_Register.Models;
using RWS_LBE_Register.Settings;

namespace RWS_LBE_Register.Helpers
{
    public class RlpNumberingHelper
    {
        private readonly AppDbContext _db;
        private readonly RlpNumberingOptions _options;

        public RlpNumberingHelper(AppDbContext db, IOptions<RlpNumberingOptions> options)
        {
            _db = db;
            _options = options.Value;
        }

        public async Task<RLPUserNumbering?> GenerateNextRLPUserNumberingAsync()
        {
            var now = DateTime.Now;
            var year = now.Year % 100;
            var month = now.Month;
            var day = now.Day;

            string nextRlpNo;
            var lastEntry = await _db.RLPUserNumbering
                .OrderByDescending(x => x.RLP_NO)
                .FirstOrDefaultAsync();

            if (lastEntry == null)
            {
                nextRlpNo = _options.RLPNODefault;
            }
            else if (!ulong.TryParse(lastEntry.RLP_NO, out var lastNo))
            {
                throw new Exception("Failed to parse last RLP_NO as integer.");
            }
            else
            {
                nextRlpNo = (lastNo + 1).ToString("D11");
            }

            var todayEntry = await _db.RLPUserNumbering
                .Where(x => x.Year == year && x.Month == month && x.Day == day)
                .OrderByDescending(x => x.RLPIDEndingNO)
                .FirstOrDefaultAsync();

            var nextEndingNo = todayEntry == null ? 1 : todayEntry.RLPIDEndingNO + 1;

            var rlpID = $"{year:D2}{month:D2}{day:D2}{nextEndingNo:D5}";

            var newRlp = new RLPUserNumbering
            {
                Year = year,
                Month = month,
                Day = day,
                RLP_ID = rlpID,
                RLP_NO = nextRlpNo,
                RLPIDEndingNO = nextEndingNo
            };

            _db.RLPUserNumbering.Add(newRlp);
            await _db.SaveChangesAsync();

            return newRlp;
        }

        public async Task<RLPUserNumbering?> GenerateNextRLPUserNumberingWithRetryAsync()
        {
            Exception? lastErr = null;

            for (int attempt = 1; attempt <= _options.MaxAttempts; attempt++)
            {
                try
                {
                    return await GenerateNextRLPUserNumberingAsync();
                }
                catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate") == true ||
                                                  ex.InnerException?.Message.Contains("UNIQUE") == true)
                {
                    lastErr = ex;
                    continue;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            throw new Exception($"Failed to generate RLP number after {_options.MaxAttempts} attempts.", lastErr);
        }
    }
}
